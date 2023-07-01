# Objective: Learn
Objective: Learn is a program made to teach children the basics of [OOP](https://en.wikipedia.org/wiki/Object-oriented_programming) with a Java-esque syntax.

## Building Objective: Learn
Objective: Learn supports Windows, Linux and MacOS, although MacOS is currently untested.
### Prerequesites
 - .NET 7

Only for compiling the Icons on Linux:
 - png2icns for Mac ICNS
 - ImageMagick for Windows ICO

### Build Commands
Windows:
```bash
dotnet build -p Platform=Windows
```
Linux:
```bash
dotnet build -p Platform=Linux
```
MacOS:
```bash
dotnet build -p Platform=MacOS
```
ICNS on Linux:
```bash
png2icns MacIcon.icns Icons/ObjectiveLearnLogo*.png

```
ICO on Linux:
```bash
convert Icons/ObjectiveLearnLogo512.png -define icon:auto-resize=256,64,48,32,16 WinIcon.ico
```