echo = ProceduralModelGenerator = Controller
xcopy /Y /C /H /Q dev\ProceduralModelGenerator\Controller\bin\Release\netcoreapp2.0\*.dll ship\bin\*.dll 1>nul
xcopy /Y /C /H /Q dev\ProceduralModelGenerator\Controller\bin\Release\netcoreapp2.0\*.exe ship\bin\*.exe 1>nul

echo = ProceduralModelGenerator = Model
xcopy /Y /C /H /Q dev\ProceduralModelGenerator\Model\bin\Release\netcoreapp2.0\*.dll ship\bin\*.dll 1>nul
xcopy /Y /C /H /Q dev\ProceduralModelGenerator\Model\bin\Release\netcoreapp2.0\*.exe ship\bin\*.exe 1>nul

echo = ProceduralModelGenerator = WindowsView
xcopy /Y /C /H /Q dev\ProceduralModelGenerator\WindowsView\bin\Release\*.dll ship\bin\*.dll 1>nul
xcopy /Y /C /H /Q dev\ProceduralModelGenerator\WindowsView\bin\Release\*.exe ship\bin\*.exe 1>nul

echo = Visualizers = VisualizerLibrary
xcopy /Y /C /H /Q dev\Visualizers\VisualizerLibrary\bin\Release\netstandard1.0\*.dll ship\bin\*.dll 1>nul
xcopy /Y /C /H /Q dev\Visualizers\VisualizerLibrary\bin\Release\netstandard1.0\*.exe ship\bin\*.exe 1>nul

echo = Visualizers = WcfVisualizerLibrary
xcopy /Y /C /H /Q dev\Visualizers\WcfVisualizerLibrary\bin\Release\*.dll ship\bin\*.dll 1>nul
xcopy /Y /C /H /Q dev\Visualizers\WcfVisualizerLibrary\bin\Release\*.exe ship\bin\*.exe 1>nul

echo = Visualizers = WpfVisualizer
xcopy /Y /C /H /Q dev\Visualizers\WpfVisualizer\bin\Release\*.dll ship\bin\*.dll 1>nul
xcopy /Y /C /H /Q dev\Visualizers\WpfVisualizer\bin\Release\*.exe ship\bin\*.exe 1>nul

echo = Data
copy /Y "toship\readme-for-shipped-app.txt" ".\ship\README.txt"
copy /Y "toship\licence-for-shipped-app.txt" ".\ship\LICENCE.txt"
copy /Y "toship\register-http-url.bat" "ship\INSTALL(run as ADMIN).bat"
copy /Y "toship\unregister-http-url.bat" "ship\UNINSTALL(run as ADMIN).bat"
xcopy /Y /C /H /Q /E toship\data\* ship\data\* 1>nul