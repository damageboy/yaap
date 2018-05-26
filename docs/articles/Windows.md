# Windows terminal support

Windows terminal support, for the purposes of Yaap is pretty complete, and in general one should really expect Yaap to work perfectly fine in cmd.exe.

In general most features don't require anthing special to display Yaap's progress bar, except for one quirk: Unicode progress bars:

Unicode progress bars use various characters in the ["Block Elements" unicode block](https://en.wikipedia.org/wiki/Block_Elements), and for some unknown reason,
no pre-packages windows font provides these characters (e.g. [Lucida Console](https://docs.microsoft.com/en-us/typography/font-list/lucida-console), [Consolas](https://docs.microsoft.com/en-us/typography/font-list/consolas) and other default windows fonts)...
Luckily not all is lost, as there are fonts that do work with cmd.exe that do provide these charchters.

The rest of this page will guide you, the poor windows user, in getting Yaap to properly display progress bars on windows **WITH** the special unicode charcters..


## Install some not-crappy fonts

Yaap has been tested to work with the following font families on Windows:
* "Hack"
* "InputMono"
* "Hasklig"
* "DejaVu Sans Mono"
* "Iosevka"

On top normal cmd.exe support, Yaap also detects [ConEmu](https://conemu.github.io/) and [Cmder](http://cmder.net/) (Which uses ConEmu internally) 
