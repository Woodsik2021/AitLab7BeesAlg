using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using AitLab7BeesAlg.Commands;
using AitLab7BeesAlg.Models;
using AitLab7BeesAlg.Models.BeesAlg;
using AitLab7BeesAlg.Models.Extension;
using AitLab7BeesAlg.ViewModels.Builders;
using Newtonsoft.Json;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace AitLab7BeesAlg.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private List<List<Chromosome>>? _lastResultList;
        private CancellationTokenSource _tokenSource = new();
        private PlotModel _plotPlotModel = new();
        private GeneticAlgorithmBuilderVm _savedGenAlgBuilderVm;


        public MessageViewModel MessageViewModel { get; } = new();
        public GeneticAlgorithmBuilderVm GenAlgBuilderVm { get; }


        public int DrawSpeed { get; set; } = 10;
        public int GenerationsPerTick { get; set; } = 10;

        public PlotModel PlotModel
        {
            get => _plotPlotModel;
            private set
            {
                _plotPlotModel = value;
                _plotPlotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Title = "X1" });
                _plotPlotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Title = "X2" });
            }
        }


        public Chromosome? LastResult { get; private set; }

        public string AlgTextLog { get; set; } = String.Empty;
        public int MaxLogSize { get; set; } = 500;

        public string AllResultsFilePath { get; set; } = $@"{Directory.GetCurrentDirectory()}\AllResults.json";
        public string LastResultLogFilePath { get; set; } = $@"{Directory.GetCurrentDirectory()}\LastResultLog.json";


        public MainViewModel()
        {
            GenAlgBuilderVm = new GeneticAlgorithmBuilderVm { MessageViewModel = MessageViewModel };
            _savedGenAlgBuilderVm = GenAlgBuilderVm;
            PlotModel = new PlotModel { Title = "F(x1,x2)" };
        }


        public ICommand FindMinCommand => new ActionCommand(_ => FindMin());
        public ICommand AnimatePlotAsyncCommand => new ActionCommand(_ => AnimatePlot(), _ => LastResult is not null);

        public ICommand StopAnimatePlotAsyncCommand =>
            new ActionCommand(_ => _tokenSource.Cancel(), _ => LastResult is not null);

        public ICommand SaveLastResultCommand => new ActionCommand(_ => SaveLastResult(), _ => LastResult is not null);

        public ICommand SaveLastResultLogCommand =>
            new ActionCommand(_ => SaveLastResultLog(), _ => LastResult is not null);


        private void FindMin()
        {
            try
            {
                _tokenSource.Cancel();
                _savedGenAlgBuilderVm = GenAlgBuilderVm.Clone();


                var geneticAlgorithm = GenAlgBuilderVm.Build();
                var func = geneticAlgorithm.ObjectiveFunction;
                _lastResultList = geneticAlgorithm.FindMinFunctionAndSave().ToList();
                LastResult = _lastResultList.Last().MinBy(chr => chr.FuncValue);

                var (stringBuilder, curGeneration, divider) =
                    (new StringBuilder(), 0, Math.Max(1, Math.Ceiling(_lastResultList.Count / (double)MaxLogSize)));
                foreach (var generationList in _lastResultList)
                {
                    if (curGeneration % divider == 0 || curGeneration == _lastResultList.Count - 1)
                        stringBuilder.Append(
                            $"Generation: {curGeneration}; Best Result - {generationList.MinBy(chromosome => chromosome.FuncValue)}{Environment.NewLine}");

                    curGeneration++;
                }

                AlgTextLog = stringBuilder.ToString();


                PlotModel = new PlotModel { Title = "F(x1,x2)" };
                PlotModel.Axes.Add(new LinearColorAxis
                {
                    Position = AxisPosition.Right, Palette = OxyPalettes.Rainbow(_lastResultList.Count),
                    Title = "Generation"
                });

                var scatterSeries = new ScatterSeries { MarkerType = MarkerType.Cross };
                scatterSeries.Points.AddRange(
                    _lastResultList.SelectMany((list, i) =>
                        list.Select(chromosome => new ScatterPoint(chromosome.X1, chromosome.X2, 4, i))).ToList());

                var xx = ArrayBuilder.CreateVector(-GenAlgBuilderVm.X1Bounds.A, -GenAlgBuilderVm.X1Bounds.B, 100);
                var yy = ArrayBuilder.CreateVector(-GenAlgBuilderVm.X2Bounds.A, -GenAlgBuilderVm.X2Bounds.B, 100);
                var peaksData = ArrayBuilder.Evaluate(func, xx, yy);

                var contourSeries = new ContourSeries
                {
                    Color = OxyColors.Black, LabelBackground = OxyColors.White,
                    RowCoordinates = xx, ColumnCoordinates = yy, Data = peaksData
                };

                PlotModel.Series.Add(contourSeries);
                PlotModel.Series.Add(scatterSeries);
                PlotModel.InvalidatePlot(true);
            }
            catch (Exception e)
            {
                MessageViewModel.Message = e.Message;
            }
        }

        private async void AnimatePlot()
        {
            try
            {
                _tokenSource.Cancel();
                _tokenSource = new CancellationTokenSource();

                var scatterSeries = PlotModel.Series.OfType<ScatterSeries>().First();
                scatterSeries.Points.ForEach(point => point.Size = 0);

                var acc = 0;
                foreach (var point in scatterSeries.Points)
                {
                    point.Size = 4;
                    if (acc % (_savedGenAlgBuilderVm.PopulationSize * GenerationsPerTick) == 0)
                    {
                        if (_tokenSource.Token.IsCancellationRequested) return;
                        var taskDelay = Task.Delay(TimeSpan.FromSeconds(1.0 / DrawSpeed), _tokenSource.Token);
                        PlotModel.InvalidatePlot(true);
                        await taskDelay;
                        await Task.Delay(50);
                    }

                    acc++;
                }

                PlotModel.InvalidatePlot(true);
            }
            catch (TaskCanceledException)
            {
            }
            catch (Exception e)
            {
                MessageViewModel.Message = e.Message;
            }
        }

        private void SaveLastResult()
        {
            var serializedResult = JsonConvert.SerializeObject(
                new
                {
                    SaveTime = DateTime.Now,
                    AlgorithmInput = _savedGenAlgBuilderVm,
                    OptimizationResult = LastResult
                },
                Formatting.Indented, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });

            try
            {
                new SimpleFileLogger(AllResultsFilePath).LogToFile(serializedResult);
            }
            catch (Exception e)
            {
                MessageViewModel.Message = e.Message;
            }
        }

        private void SaveLastResultLog()
        {
            var serializedResult = JsonConvert.SerializeObject(
                new
                {
                    SaveTime = DateTime.Now,
                    AlgorithmInput = _savedGenAlgBuilderVm,
                    OptimizationLog = _lastResultList?.Select((curGenerationChromosomes, curGeneration) =>
                        new
                        {
                            Generation = curGeneration,
                            BestResult = curGenerationChromosomes.MinBy(chr => chr.FuncValue),
                            AllChromosomes = curGenerationChromosomes
                        }),
                    OptimizationResult = LastResult
                },
                Formatting.Indented, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });

            try
            {
                new SimpleFileLogger(LastResultLogFilePath).LogToFileTruncate(serializedResult);
            }
            catch (Exception e)
            {
                MessageViewModel.Message = e.Message;
            }
        }


        public event PropertyChangedEventHandler? PropertyChanged;
    }
}