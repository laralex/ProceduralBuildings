echo = ProceduralModelGenerator = Controller
xcopy /Y /C /H /Q dev\ProceduralModelGenerator\Controller\bin\Release\netcoreapp2.0\*.dll ship\bin\*.dll > nil 
xcopy /Y /C /H /Q dev\ProceduralModelGenerator\Controller\bin\Release\netcoreapp2.0\*.exe ship\bin\*.exe > nil

echo = ProceduralModelGenerator = Model
xcopy /Y /C /H /Q dev\ProceduralModelGenerator\Model\bin\Release\netcoreapp2.0\*.dll ship\bin\*.dll > nil
xcopy /Y /C /H /Q dev\ProceduralModelGenerator\Model\bin\Release\netcoreapp2.0\*.exe ship\bin\*.exe > nil

echo = ProceduralModelGenerator = WindowsView
xcopy /Y /C /H /Q dev\ProceduralModelGenerator\WindowsView\bin\Release\*.dll ship\bin\*.dll > nil
xcopy /Y /C /H /Q dev\ProceduralModelGenerator\WindowsView\bin\Release\*.exe ship\bin\*.exe > nil

echo = Visualizers = VisualizerLibrary
xcopy /Y /C /H /Q dev\Visualizers\VisualizerLibrary\bin\Release\netstandard1.0\*.dll ship\bin\*.dll > nil
xcopy /Y /C /H /Q dev\Visualizers\VisualizerLibrary\bin\Release\netstandard1.0\*.exe ship\bin\*.exe > nil

echo = Visualizers = WcfVisualizerLibrary
xcopy /Y /C /H /Q dev\Visualizers\WcfVisualizerLibrary\bin\Release\*.dll ship\bin\*.dll > nil
xcopy /Y /C /H /Q dev\Visualizers\WcfVisualizerLibrary\bin\Release\*.exe ship\bin\*.exe > nil

echo = Visualizers = WpfVisualizer
xcopy /Y /C /H /Q dev\Visualizers\WpfVisualizer\bin\Release\*.dll ship\bin\*.dll > nil
xcopy /Y /C /H /Q dev\Visualizers\WpfVisualizer\bin\Release\*.exe ship\bin\*.exe > nil

echo = Data
copy /Y readme-for-shipped-app.txt ship\readme.txt > nil