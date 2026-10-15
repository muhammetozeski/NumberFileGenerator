# NumberFileGenerator

![Windows x64](https://img.shields.io/badge/Windows-x64-0078D6)
![.NET 10](https://img.shields.io/badge/.NET-10-512BD4)
![C# 14](https://img.shields.io/badge/C%23-14-239120)

A small console application that quickly creates large text files containing consecutive numbers.
Enter a size such as `1,5`; the program writes `1 2 3 4 ` onwards beside its executable.

- Accepts a decimal comma or decimal point. One GB means 1,073,741,824 bytes.
- Uses an 8 MiB memory buffer and increments ASCII digits directly without allocating a string per number.
- Creates exactly the requested whole-byte size. Any space after the final complete number is padded
  with at most 19 spaces; a number is never cut in half.
- Existing files are preserved; repeated requests receive a numbered filename suffix.
- Creates no folders, settings, logs, language files or extraction cache. Console messages follow the
  system's Turkish or English language. Only the requested text file is written.
- Ctrl+C cancels at a buffer boundary. A clearly reported partial output is retained after cancellation
  or a write error.

## Download and use

Download the executable from the [latest release](https://github.com/muhammetozeski/NumberFileGenerator/releases/latest).
Requires the **.NET 10 x64 runtime** on Windows. This is a framework-dependent single executable;
the runtime is not bundled.

Run the EXE, enter the requested GB amount and press Enter. The output directory is the executable's
directory, even when the program is launched with a different working directory.

The executable is digitally signed. To let Windows verify the signature, run `Install-Certificate.cmd`
from `SignatureTrust.zip` once. The program runs without it; only the signature stays unverified.

## Build

With the .NET 10 SDK installed, run `./build.ps1`. The only distribution file is written to
`publish/FrameworkDependent/NumberFileGenerator.exe`. Native libraries and content extraction are
disabled; no runtime cache directory is used.
