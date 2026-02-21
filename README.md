<div align="center">

  # CS2 AUTO-B-HOP by CS1337BOYS
  **The Most Scientifically Accurate, Sub-Tick Optimized Bunnyhop & Long-Jump Engine for Counter-Strike 2**

  [![License](https://img.shields.io/badge/License-MIT-blue.svg?style=for-the-badge&color=252525)](https://opensource.org/licenses/MIT)
  [![Steam Group](https://img.shields.io/badge/Steam-CS1337BOYS-black?logo=steam&style=for-the-badge)](https://steamcommunity.com/groups/CS1337BOYS)
</div>

<br>

Counter-Strike 2 shifted movement entirely to a Sub-Tick architecture. Standard mouse wheel scrolling or random macro spamming no longer works flawlessly—they intentionally trigger an anti-spam penalty if inputs exceed 15.625ms or miss the Sub-Tick overlap window.

**CS2 AUTO-B-HOP by CS1337BOYS** solves this mathematically. Built on absolute raw Windows Hooks (`SetWindowsHookEx`) and CPU SpinWait timers (`Stopwatch`), this engine injects perfect `0.00ms` latency inputs directly into the OS queue, fully bypassing the CS2 sub-tick penalty while guaranteeing maximum possible forward momentum (Velocity).

---

## ⚡ Key Features

- **Sub-Tick Physics Bypass:** Calculates the precise 7.03ms jump hold window (Sub-Tick overlap subtraction) needed to avoid the dreaded Source 2 stamina penalty.
- **Perfect Duck-Jump (Long-Jump):** Simulates an idealized Long-Jump by pressing Crouch (`Ctrl`) exactly `0.5ms` *before* the Jump (`Space`) input. This flexes the character's collision hull, adding raw horizontal velocity not achievable by human hands. Space releases quickly, while Crouch is held in the air for `15ms` to maximize glide distance.
- **Hyper-Threaded Engine:** Uses `.NET GCLatencyMode.SustainedLowLatency` and `ProcessPriorityClass.High` to freeze Windows Garbage Collection and prioritize the script thread to `Highest`.
- **Zero Input Lag:** Utilizes exact `SpinWait` polling instead of lazy `Thread.Sleep()`, ensuring millisecond accuracy without OS sleep jitter.
- **Premium CS2 UI:** Beautiful, compact, borderless dark-mode WinForms application with GDI+ diagonal renders replicating the official CS2 aesthetic.

## 🚀 Installation & Usage

1. **Download:** Grab the latest `CS2Bhop.exe` from the [Releases](https://github.com/Adiru3/CS2-Auto-Bhop/releases) tab.
2. **Run:** Open `CS2Bhop.exe`. (The application requires Administrator privileges to inject keyboard hooks reliably).
3. **Configure:** Check the **"Enable Duck-Jump"** box if you want maximum forward jump distance. Uncheck for standard flat sub-tick jumping.
4. **Start:** Click `START BHOP`. The script will now listen seamlessly in the background.
5. **Play:** Go into CS2, run, and simply **Hold Spacebar**.

## 🛠 Compilation (For Developers)

Requires the .NET Framework 4.0 Compiler (`csc.exe`).

```cmd
C:\Windows\Microsoft.NET\Framework\v4.0.30319\csc.exe /target:winexe /win32manifest:app.manifest /out:CS2Bhop.exe CS2BhopUI.cs
```

## 🌐 Official Website & Web Mini-Game
Visit our official website to test your raw Bhop reflex skills in an interactive 2D Canvas Mini-Game!
[CS1337BOYS Official Website](https://adiru3.github.io/CS2-Auto-Bhop/)

## 🤝 Support
If this script helped you hit that insane 300-velocity clip, consider dropping a star ⭐ or supporting the development!

[Donate here!](https://adiru3.github.io/Donate/) | [Join our Steam Group!](https://steamcommunity.com/groups/CS1337BOYS)

> **Disclaimer:** This software simulates keyboard strokes via standard `user32.dll` APIs and does not read or inject into CS2 memory (`client.dll`). Use external hardware scripts/macros at your own risk on VAC/FaceIT servers.




