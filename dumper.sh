#!/usr/bin/env bash
set -euo pipefail  # Exit on error, undefined vars, pipeline failures

# =======================================================================
# Super Battle Golf Managed DLL Dumper + Git Auto-Push + Auto-Tagging
# =======================================================================

# CONFIG
REPO_DIR="/home/soap/Downloads/SBG Assemblies/Game Dump"
GAME_FOLDER="SuperBattleGolf"
DECOMPILED_SUBDIR="${REPO_DIR}/${GAME_FOLDER}/Decompiled"
MANAGED_DIR="/home/soap/.local/share/Steam/steamapps/common/Super Battle Golf/Super Battle Golf_Data/Managed"
MIN_DLL_SIZE=100000

# === FORCE TO REPO ROOT ===
echo "Changing to repo root: $REPO_DIR"
cd "$REPO_DIR" || { echo "FATAL: Cannot cd to $REPO_DIR"; exit 1; }

# Verify git repo
if [ ! -d ".git" ]; then
  echo "FATAL: .git not found — this is not a git repo!"
  exit 1
fi

echo "Current dir (should be repo root): $(pwd)"
git rev-parse --is-inside-work-tree || { echo "FATAL: git does not recognize this as a repo"; exit 1; }

# === CHECKS ===
if ! command -v ilspycmd &> /dev/null; then
  echo "Error: ilspycmd not found. Install: dotnet tool install --global ilspycmd"
  exit 1
fi

# === CREATE STRUCTURE ===
mkdir -p "$DECOMPILED_SUBDIR"

# === DUMP TO TEMP ===
TEMP_DUMP_DIR=$(mktemp -d -t sbg-dump-XXXXXXXX)
echo "Dumping to temp: $TEMP_DUMP_DIR"

cd "$MANAGED_DIR" || { echo "Cannot access Managed dir"; rm -rf "$TEMP_DUMP_DIR"; exit 1; }

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

# === MOVE BACK TO REPO ===
echo "Moving dump to $DECOMPILED_SUBDIR ..."
cd "$REPO_DIR" || { echo "FATAL: Cannot cd back to repo root"; rm -rf "$TEMP_DUMP_DIR"; exit 1; }
rsync -a --delete "$TEMP_DUMP_DIR/" "$DECOMPILED_SUBDIR/" || {
  echo "rsync failed (check paths/permissions)"
  rm -rf "$TEMP_DUMP_DIR"
  exit 1
}

rm -rf "$TEMP_DUMP_DIR"

# === GIT OPERATIONS ===
echo "Git operations from: $(pwd)"

git add . || { echo "git add failed"; exit 1; }

COMMIT_MSG="Updated Super Battle Golf managed decomp dump - $(date +'%Y-%m-%d %H:%M')"

if git diff --quiet --exit-code --cached; then
  echo "No changes to commit (dump identical)"
else
  git commit -m "$COMMIT_MSG" || { echo "Commit failed"; exit 1; }

  TAG_NAME="sbg-dump-$(date +'%Y%m%d-%H%M')"
  git tag "$TAG_NAME" -m "$COMMIT_MSG"

  git push origin main
  git push origin "$TAG_NAME"

  echo "Tagged as: $TAG_NAME"
  echo "Pushed to: https://github.com/8ktv/Game-Dumps/tags"
fi

echo ""
echo "Done! Dump in: $DECOMPILED_SUBDIR"
echo "Quick search:"
echo "  grep -r -i 'golf\|ball\|player\|sabotage\|power\|swing' \"$DECOMPILED_SUBDIR\""
