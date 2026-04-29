<p align="center">
  <img src="src/Mercury/Assets/Logo-100.png" alt="Mercury Logo" width="128" height="128" />
</p>

<h1 align="center">Mercury</h1>

<p align="center">
  A free, cross-platform music player that streams music directly from YouTube Music.
</p>

<p align="center">
  <img src="https://img.shields.io/badge/Platform-Windows%20%7C%20macOS%20%7C%20Linux-lightgrey" alt="Platforms" />
</p>

---

## What is Mercury?

Mercury is a native music player that lets you search, stream, and enjoy music from YouTube Music — no browser needed. It runs on Windows, macOS, and Linux with a clean, modern interface.

## Features

- **Search & Stream** — Find and play any song, album, artist, or playlist from YouTube Music
- **Synchronized Lyrics** — Follow along with real-time synced lyrics as the music plays
- **Queue & Playback Controls** — Build your queue, shuffle, repeat, skip, and seek
- **Home Feed** — Browse recommended and trending music right from the home page
- **Cross-Platform** — One app for Windows, macOS, and Linux
- **Native** — No Electron, no WebView — just a fast native desktop app

## Screenshots

*Coming soon*

## Download & Install

> Mercury is currently in active development. Pre-built releases will be available soon.

### Build from Source

You will need the [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) and Git installed.

```bash
git clone --recurse-submodules https://github.com/Aengstlicher1/Mercury-Avalonia.git
cd Mercury-Avalonia
dotnet run --project src/Mercury
```

> **Important:** The `--recurse-submodules` flag is required to pull in the Core library. If you forgot it:
> ```bash
> git submodule update --init --recursive
> ```

## How It Works

Mercury uses [Mercury.Core](https://github.com/Aengstlicher1/Mercury.Core) under the hood to handle all the heavy lifting — searching YouTube Music, resolving audio streams, and fetching lyrics. The desktop app itself is built with [Avalonia UI](https://avaloniaui.net/) and simply provides the interface and playback experience.

## Contributing

Contributions are welcome! Fork the repo, create a branch, and open a Pull Request.

If your changes involve the core logic (search, streaming, lyrics), head over to the [Mercury.Core](https://github.com/Aengstlicher1/Mercury.Core) repository instead.

## License

This project is licensed under the MIT License.

---

<p align="center">
  Built with love using <a href="https://avaloniaui.net/">Avalonia UI</a>
</p>
