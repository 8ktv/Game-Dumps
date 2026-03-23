#!/usr/bin/env bash
# =======================================================================
# Super Battle Golf Managed DLL Dumper + Git Auto-Push + Auto-Tagging
# =======================================================================

# === CONFIG ===
REPO_DIR="/home/soap/Downloads/SBG Assemblies/Game Dump"           # Must be the git repo root
GAME_FOLDER="SuperBattleGolf"
DECOMPILED_SUBDIR="$REPO_DIR/$GAME_FOLDER/Decompiled"              # Absolute path
MANAGED_DIR="/home/soap/.local/share/Steam/steamapps/common/Super Battle Golf/Super Battle Golf_Data/Managed"
MIN_DLL_SIZE=100000

# === FORCE CD TO REPO ===
if ! cd "$REPO_DIR" 2>/dev/null; then
  echo "Error: Cannot cd to repo directory: $REPO_DIR"
  echo "Check if the path is correct and exists."
  exit 1
fi

if [ ! -d ".git" ]; then
  echo "Error: $REPO_DIR is not a git repository!"
  exit 1
fi

# === CHECKS ===
if ! command -v ilspycmd &> /dev/null; then
  echo "Error: ilspycmd not found. Install with:"
  echo "  dotnet tool install --global ilspycmd"
  exit 1
fi

# === CREATE STRUCTURE ===
mkdir -p "$DECOMPILED_SUBDIR"

# === DUMP TO TEMP FOLDER ===
TEMP_DUMP_DIR=$(mktemp -d -t sbg-dump-XXXXXXXX)
echo "Dumping to temporary folder: $TEMP_DUMP_DIR"

cd "$MANAGED_DIR" || { echo "Cannot access Managed dir!"; rm -rf "$TEMP_DUMP_DIR"; exit 1; }

for dll in *.dll; do
  if [ -f "$dll" ] && [ $(stat -c %s "$dll") -gt $MIN_DLL_SIZE ]; then
    echo "=== Dumping $dll ==="
    DLL_NAME="${dll%.dll}"
    OUT_DIR="$TEMP_DUMP_DIR/$DLL_NAME"
    mkdir -p "$OUT_DIR"
    ilspycmd "$dll" -p -o "$OUT_DIR" 2> "$OUT_DIR/error.log"
    if [ $? -eq 0 ] && ls "$OUT_DIR"/*.cs >/dev/null 2>&1; then
      echo "Success: $DLL_NAME dumped"
    else
      echo "Failed or empty for $dll — check $OUT_DIR/error.log"
      rmdir "$OUT_DIR" 2>/dev/null || true
    fi
  fi
done

# === MOVE TO REPO (overwrite old dump) ===
echo "Moving new dump to $DECOMPILED_SUBDIR ..."
rsync -a --delete "$TEMP_DUMP_DIR/" "$DECOMPILED_SUBDIR/" || {
  echo "rsync failed! Check paths and permissions."
  rm -rf "$TEMP_DUMP_DIR"
  exit 1
}

# Clean up temp
rm -rf "$TEMP_DUMP_DIR"

# === GIT COMMIT, TAG & PUSH ===
echo "Committing, tagging and pushing to private repo..."

git add . || { echo "git add failed"; exit 1; }

COMMIT_MSG="Updated Super Battle Golf managed decomp dump - $(date +'%Y-%m-%d %H:%M')"
if git commit -m "$COMMIT_MSG" 2>/dev/null; then
  # Only tag and push if commit succeeded (there were changes)
  TAG_NAME="sbg-dump-$(date +'%Y%m%d-%H%M')"
  git tag "$TAG_NAME" -m "$COMMIT_MSG"
  git push origin main
  git push origin "$TAG_NAME"
  echo "Tagged as: $TAG_NAME"
else
  echo "No changes to commit (dump identical to previous)."
fi

echo ""
echo "Done! Latest dump in:"
echo "  $DECOMPILED_SUBDIR"
echo ""
echo "View tags: https://github.com/8ktv/Game-Dumps/tags"
echo ""
echo "Quick search:"
echo "  grep -r -i 'golf\|ball\|player\|sabotage\|power\|swing' \"$DECOMPILED_SUBDIR\""
