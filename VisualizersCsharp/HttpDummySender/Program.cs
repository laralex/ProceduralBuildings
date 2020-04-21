using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.ServiceModel;
using System.Text;

using System.Threading;
using System.Windows;
using VisualizerLibrary;
using WpfVisualizer;
using WpfVisualizer.Remote;

namespace HttpDummySender
{

    //[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
    public class VisualizerServiceClient : ClientBase<IVisualizerService>, IVisualizerService
    {
        public string GetDescription()
        {
            return base.Channel.GetDescription();
        }

        public void PrepareForModel(ModelDataType modelType)
        {
            base.Channel.PrepareForModel(modelType);
        }

        public void Shutdown()
        {
            base.Channel.Shutdown();
        }

        public void VisualizeModel(Stream model)
        {
            base.Channel.VisualizeModel(model);
        }

        public void VisualizeModel(Stream model, ModelDataType type = ModelDataType.OBJ)
        {
            base.Channel.VisualizeModel(model, type);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("> launch");
            var modelFiles = Directory.GetFiles(Path.Combine(Environment.CurrentDirectory, "data"));
            var rng = new Random();
            var client = new VisualizerServiceClient();
            Console.WriteLine("> client started");

            for (int i = 0; i < 20; ++i)
            {
                var randomModel = modelFiles[rng.Next(modelFiles.Length)];
                var model = File.OpenRead(randomModel);
                Console.WriteLine($"{i}> sending file {randomModel}");
                try
                {
                    client.PrepareForModel(ModelDataType.OBJ);
                    client.VisualizeModel(File.OpenRead(randomModel));
                    Console.WriteLine($"{i}> sent file {randomModel}");
                }
                catch(Exception e)
                {
                    MessageBox.Show($"{i}> e: {e.Message}");
                }
                Thread.Sleep(10000);
            }
            Console.WriteLine("> shutdown");
        }
    }
}
