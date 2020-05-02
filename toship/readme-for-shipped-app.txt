Applications require Administrator rights to work properly. If you want to allow them
run with such rights without bothering you with popup dialogs, you may install the application.
For this, launch the INSTALL.bat application with admin rights. You can inspect this file,
it's doesn't do any harm, but registers some HTTP ports. You can uninstall the application at any time.

You don't have to install the program, but then launch the application with admin rights yourself.

= = = = =

To start working with the procedural generator, open a file:
bin/BuildingsGenerator.exe

From there on, you can tweak generation configuration and start the generation. 
A generated model (without textures) can be visualized by other application
if it properly communicates with the HTTP service:
http://localhost:64046/visualizationControllerService

An example visualizer is implemented and it's a program:
bin/WpfVisualizer.exe