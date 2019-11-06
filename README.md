# ChipEight.Net

During my Christmas break 2018 I spent a couple of days hacking on a Chip 8 emulator.

I thought it would be fun to throw it on github but decided to wait until SFML.Net got 
their nuget release sorted and by the time they had I was busy with life.

Found myself some downtime so I have done a little maintenance, now I can call 
it done and forget about it.

## Usage

```bash
$ .\ChipEight.Net .\INVADERS.ch8
```

Or just drag the INVADERS file onto the exe in the explorer window to save opening a shell.

For invaders the keys to play are 

__Q__: left  
__W__: shoot  
__E__: right  

## Screenshots

![Title image](misc/title.png?raw=true "Title screen")
![Gameplay image](misc/gameplay.png?raw=true "Gameplay")

## Publishing
```bash
# From sln dir
$ dotnet publish .\src\ChipEight.Net\ChipEight.Net.csproj `
-r win-x64 -c Release /p:PublishSingleFile=true /p:Version=0.1.0 -o dist/
```