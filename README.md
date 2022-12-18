 # FEML
 
 FEML is a simple markup language for describing various configuration settings. It was designed to be easily readable and writable by both humans and machines.
 
 ## Syntax
 
 FEML documents consist of a series of statements, which are used to define variables and their values. Variables can be numbers, strings, arrays, or objects.
 
 ### Numbers
 
 Numbers in FEML are represented as plain digits. For example:
 
 `number = 69;` 
 
 ### Strings
 
 Strings in FEML are represented as a sequence of characters surrounded by single or double quotes (`'`, `"`). For example:
 
 `message = ['I', 'love', 'FEML'];` 
 
 ### Arrays
 
 Arrays in FEML are represented as a list of values surrounded by square brackets (`[` and `]`). For example:
 
 `presets = [1, 2, 3, 4];` 
 
 ### Objects
 
 Objects in FEML are represented as a collection of name-value pairs surrounded by curly braces (`{` and `}`). For example:
 
 ```
 graphics = { 
   msaaLevel = 4;
   shadowQuality = 2;
   enableBloom = true;
   presets = [1, 2, 3, 4];
   advanced = {
       fsr = 1.6; 
   }; 
 };
 ```
 
 ## Examples
 
 Here is an example of a complete FEML document representing various configuration settings:
 
 ```
 /* FEML! */
 number = 69;
 message = ['I', 'love', 'FEML']; 

 graphics = { 
   msaaLevel = 4; 
   shadowQuality = 2; 
   enableBloom = true; 
   presets = [1, 2, 3, 4]; 
   advanced = { 
       renderScale = 1.6;
   }; 
 }; 

 audio = { 
   sfxLevel = 1; 
   musicLevel = 2; 
 };
 ```