# Reverse-Snake Game
[![AUR](https://img.shields.io/aur/license/yaourt.svg)](https://github.com/ezhivitsa/Reverse-Snake/blob/master/LICENSE)

Reverse-Snake is the logic-game where you collect cubes with a -1 point and should not to get lost in walls and your tail.
>**Note:**  you can collect points only on last or 2rd remaining steps of the snake tail

## Primary technologies
In our game we used:
 - Unity3D
 - LeoECS - Entity Component System framework
 - Android SDK tool
 - JDK

## How to build .apk file
1. Ensure that you installed **Android Build Support** when you installed Unity
2. Install **Android SDK tool** and **JDK-8** 
>**Note:** Recommended to install with Visual Studio 
3. In **Edit -> Preferences -> External Tools** paste a path to the sdk and jdk
4. In **Edit -> Project Settings -> Player** write a Company Name and a Product Name
5. In **File -> Build Settings -> Platforms** select Android and ensure that chosen **Build System** is **Internal**
6.  Connect your phone to a computer (USB debugging should be enabled on your phone) and press **Build And Run** 

## LICENSE
GNU General Public License v3.0
See [LICENSE](https://github.com/ezhivitsa/Reverse-Snake/blob/master/LICENSE) to see the full text.
