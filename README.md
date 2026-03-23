# Game Dump Archive

This is a private repo for my local reverse-engineering notes, decompiled snippets, and strings dumps from various games (personal use/educational only).

## Super Battle Golf (SBG) Section
- **Source**: Local Steam install on Fedora Linux
- **Tools used**: ilspycmd for managed .NET DLLs (SharedAssembly.dll, Mirror.dll, etc.), strings command for GameAssembly.dll
- **Status**: Partial managed decomp (no full IL2CPP source due to stripped metadata)
- **Disclaimers**:
  - For offline personal reference/mod prototyping only.
  - No redistribution, public sharing, or commercial use.
  - Derived from copyrighted game code — respect EULA.

## Folder Structure
- Decompiled/ → ilspycmd project outputs (.cs files per DLL)
- Strings/ → raw extracted text from binaries
- Notes/ → greps, class/method findings, mod ideas

Commit history tracks dump updates after game patches.
