using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using EpidSimulation.Models;
using EpidSimulation.Utils;
using EpidSimulation.Views;
using Excel =  Microsoft.Office.Interop.Excel;

namespace EpidSimulation.ViewModels
{
    public partial class VMF_Workplace : VM_BASIC
    {
        private Simulation _simulation;
        public Simulation CurSimulation
        {
            set => _simulation = value;
            get => _simulation;
        }

        private Config _config;
        public Config Config
        {
            set
            {
                _config = value;
                Human.Config = value;
            }
            get => _config;
        }

        private Canvas _canvasMap = new Canvas();
        public Canvas CanvasMap
        {
            get => _canvasMap;
        }

        private LinkedList<VM_Human> _figures;
        private QuadTree _curNode;
        
        private DispatcherTimer _timer;
        private bool _statusTimer = false;

        private bool _statusDebug = false;

        private bool _statusExcel = false;
        private int _curLine = 2;
        private Excel.Application _excelApp;

        private Rectangle _regionBackground;
        private Line _lineVMP;
        private Line _lineHMP;

        private ScaleTransform _scaleNode;
        
        public double WindowWidth { set; get; }
        public double WindowHeight { set; get; }

        #region BitMapImages

        private BitmapImage _biSocDistHidden = new BitmapImage();
        private BitmapImage _biSocDistVisible = new BitmapImage();

        private BitmapImage _biSocDist;
        public BitmapImage BISocDist 
        {
            set
            {
                _biSocDist = value;
                OnPropertyChanged();
            } 
            get => _biSocDist; 
        }

        private BitmapImage _biStart = new BitmapImage();
        private BitmapImage _biStop = new BitmapImage();

        private BitmapImage _biStartStopSim;
        public BitmapImage BIStartStopSim
        {
            set
            {
                _biStartStopSim = value;
                OnPropertyChanged();
            }
            get => _biStartStopSim;
        }

        private BitmapImage _biExcelOn = new BitmapImage();
        private BitmapImage _biExcelOff = new BitmapImage();

        private BitmapImage _biExcel;
        public BitmapImage BIExcel
        {
            set
            {
                _biExcel = value;
                OnPropertyChanged();
            }
            get => _biExcel;
        }
        #endregion

        public VMF_Workplace()
        {
            #region InitBitMapImages

            _biExcelOn.BeginInit();
            _biExcelOn.UriSource = new Uri("C:/Users/User/Desktop/Diplom/Application/EpidSimulation/EpidSimulation/Resource/ExcelLogoOn.png");
            _biExcelOn.EndInit();

            _biExcelOff.BeginInit();
            _biExcelOff.UriSource = new Uri("C:/Users/User/Desktop/Diplom/Application/EpidSimulation/EpidSimulation/Resource/ExcelLogoOff.jpg");
            _biExcelOff.EndInit();

            _biSocDistVisible.BeginInit();
            _biSocDistVisible.UriSource = new Uri("C:/Users/User/Desktop/Diplom/Application/EpidSimulation/EpidSimulation/Resource/SocDistVisible.jpg");
            _biSocDistVisible.EndInit();

            _biSocDistHidden.BeginInit();
            _biSocDistHidden.UriSource = new Uri("C:/Users/User/Desktop/Diplom/Application/EpidSimulation/EpidSimulation/Resource/SocDistHidden.jpg");
            _biSocDistHidden.EndInit();

            _biStart.BeginInit();
            _biStart.UriSource = new Uri("C:/Users/User/Desktop/Diplom/Application/EpidSimulation/EpidSimulation/Resource/play.png");
            _biStart.EndInit();

            _biStop.BeginInit();
            _biStop.UriSource = new Uri("C:/Users/User/Desktop/Diplom/Application/EpidSimulation/EpidSimulation/Resource/pause.png");
            _biStop.EndInit();
            #endregion

            BISocDist = _biSocDistVisible;
            BIStartStopSim = _biStart;
            BIExcel = _biExcelOn;

            Config = new Config();
            CurSimulation = new Simulation();  
            CurSimulation.SetNewSession(
                100, 100,
                0,0,0,0,
                0,0,0,0,
                0,0,0,0,
                0,0,0,0,
                0,0,0,0,
                0,0,0,0,
                false);
            _curNode = CurSimulation.Root;
            CreateAndAddFigures(CurSimulation.People);

            _timer = new DispatcherTimer();
            _timer.Tick += new EventHandler(TimerTick);
            _timer.Interval = new TimeSpan(10000);
            _statusTimer = false;

            _regionBackground = new Rectangle
            {
                Width = CurSimulation.SizeWidth,
                Height = CurSimulation.SizeHeight,
                Fill = Brushes.LightGray,
                Stroke = Brushes.Black,
                StrokeThickness = 0.1
            };
            Canvas.SetLeft(_regionBackground, 0);
            Canvas.SetTop(_regionBackground, 0);
            CanvasMap.Children.Add(_regionBackground);

            _lineVMP = new Line
            {
                X1 = CurSimulation.SizeWidth / 2,
                Y1 = 0,
                X2 = CurSimulation.SizeWidth / 2,
                Y2 = CurSimulation.SizeHeight,
                Stroke = Brushes.Black,
                StrokeThickness = 0.1
            };
            CanvasMap.Children.Add(_lineVMP);

            _lineHMP = new Line
            {
                X1 = 0,
                Y1 = CurSimulation.SizeHeight / 2,
                X2 = CurSimulation.SizeWidth,
                Y2 = CurSimulation.SizeHeight / 2,
                Stroke = Brushes.Black,
                StrokeThickness = 0.1
            };
            CanvasMap.Children.Add(_lineHMP);

            _scaleNode = new ScaleTransform();
            CanvasMap.RenderTransform = _scaleNode;

            _scaleNode.ScaleX = Math.Min(WindowWidth / CurSimulation.SizeWidth, WindowHeight / CurSimulation.SizeHeight);
            _scaleNode.ScaleY = Math.Min(WindowWidth / CurSimulation.SizeWidth, WindowHeight / CurSimulation.SizeHeight);
        }

        #region WorkWithFigureHuman

        private void CreateAndAddFigures(LinkedList<Human> people)
        {
            _figures = new LinkedList<VM_Human>();
            foreach (Human human in people)
            {
                VM_Human figure = new VM_Human(human);
                _figures.AddFirst(figure);
            }

            foreach (VM_Human figure in _figures)
            {
                if (figure.SocDistCircle != null)
                    CanvasMap.Children.Add(figure.SocDistCircle);
            }

            foreach (VM_Human figure in _figures)
            {
                CanvasMap.Children.Add(figure.CondCircle);
                if (figure.MaskCircle != null)
                    CanvasMap.Children.Add(figure.MaskCircle);

            }
        }

        private void ClearDeads()
        {
            for (LinkedListNode<VM_Human> figure = _figures.First; figure != null;)
            {
                if (figure.Value.CondCircle.Fill == Brushes.DarkGray)
                {
                    _canvasMap.Children.Remove(figure.Value.CondCircle);
                    if (figure.Value.MaskCircle != null)
                    {
                        _canvasMap.Children.Remove(figure.Value.MaskCircle);
                    }
                    if (figure.Value.SocDistCircle != null)
                    {
                        _canvasMap.Children.Remove(figure.Value.SocDistCircle);
                    }
                    LinkedListNode<VM_Human> temp = figure;
                    figure = figure.Next;
                    _figures.Remove(temp);
                }
                else
                {
                    figure = figure.Next;
                }
            }
        }

        private void ClearMap()
        {
            if (_figures != null)
            {
                foreach (VM_Human figure in _figures)
                {
                    _canvasMap.Children.Remove(figure.CondCircle);
                    if (figure.MaskCircle != null)
                    {
                        _canvasMap.Children.Remove(figure.MaskCircle);
                    }
                    if (figure.SocDistCircle != null)
                    {
                        _canvasMap.Children.Remove(figure.SocDistCircle);
                    }
                }

                _figures = null;
            }
        }

        private void MakeVisibleNodeHumans()
        {
            LinkedList<LinkedList<Human>> tempList = new LinkedList<LinkedList<Human>>();
            _curNode.GetListObjects(tempList);

            foreach (VM_Human figure in _figures)
            {
                bool vis = false;
                foreach (LinkedList<Human> l in tempList)
                {
                    if (l.Find(figure.human) != null)
                    {
                        vis = true;
                    }
                    if (vis)
                        break;
                }

                if (vis)
                {
                    figure.SetVisible(0, _statusDebug);
                }
                else
                {
                    figure.SetVisible(1, _statusDebug);
                }
            }
        }

        #endregion

        #region SimulationParams

        private int _sizeWidth = 100;
        public int SizeWidth 
        { 
            set => _sizeWidth = value > 10 ? value : 10; 
            get => _sizeWidth; 
        }
        private int _sizeHeight = 100;
        public int SizeHeight 
        { 
            set => _sizeHeight = value > 10 ? value : 10; 
            get => _sizeHeight; 
        }

        public int AmountZdNothing { set; get; }
        public int AmountZdMask { set; get; }
        public int AmountZdSocDist { set; get; }
        public int AmountZdAll { set; get; }


        public int AmountIncNothing { set; get; }
        public int AmountIncMask { set; get; }
        public int AmountIncSocDist { set; get; }
        public int AmountIncAll { set; get; }


        public int AmountProdNothing { set; get; }
        public int AmountProdMask { set; get; }
        public int AmountProdSocDist { set; get; }
        public int AmountProdAll { set; get; }


        public int AmountClinNothing { set; get; }
        public int AmountClinMask { set; get; }
        public int AmountClinSocDist { set; get; }
        public int AmountClinAll { set; get; }


        public int AmountVzdNothing { set; get; }
        public int AmountVzdMask { set; get; }
        public int AmountVzdSocDist { set; get; }
        public int AmountVzdAll { set; get; }


        public int AmountAsymptNothing { set; get; }
        public int AmountAsymptMask { set; get; }
        public int AmountAsymptSocDist { set; get; }
        public int AmountAsymptAll { set; get; }

        #endregion

        // Итерация основного таймера 
        private void TimerTick(object sender, EventArgs e)
        {
            CurSimulation.Iterate();
            if (CurSimulation.StatusChanges)
                WriteLineExcel();
            if (_statusExcel && CurSimulation.IterFinal != 0)
            {
                _timer.Stop();
                _statusTimer = false;
                BIStartStopSim = _biStart;
                _curLine--;
                CreateDiagramm("A1", "H" + _curLine.ToString(), "Диаграмма эпид процесса", "График количества заболевших", 1);
            }
            ClearDeads();
        }

        private bool _statusCollision = false;
        public bool StatusCollision
        {
            set
            {
                _statusCollision = value;
                CurSimulation.OnOffCollision(value);
            }
            get => _statusCollision;
        }

        // Включить/Выключить симуляцию
        public ICommand bStartStopSimulation_Click => new RelayCommand(_DoStartStopSimulation_Click, _AlwaysTrue);
        private void _DoStartStopSimulation_Click()
        {
            if (_statusTimer)
            {
                _timer.Stop();
                _statusTimer = false;
                BIStartStopSim = _biStart;
            }
            else
            {
                _timer.Start();
                _statusTimer = true;
                BIStartStopSim = _biStop;
            }
        }

        // Включить/Выключить отображение соц дистанции
        public ICommand bOnOffDebug_Click => new RelayCommand(_DoOnOffDebug_Click, _AlwaysTrue);
        private void _DoOnOffDebug_Click()
        {
            if (!_statusDebug)
            {
                foreach (VM_Human figure in _figures)
                {
                    if (figure.human.SocDist && figure.CondCircle.Visibility == 0)
                        figure.SocDistCircle.Visibility = Visibility.Visible;
                }
                BISocDist = _biSocDistHidden;
                _statusDebug = true;
            }
            else
            {
                foreach (VM_Human figure in _figures)
                {
                    if (figure.SocDistCircle != null)
                        figure.SocDistCircle.Visibility = Visibility.Hidden;
                }
                BISocDist = _biSocDistVisible;
                _statusDebug = false;
            }
        }

        public ICommand bCreateConfig_Click => new RelayCommand(_DoCreateConfig_Click, _AlwaysTrue);
        private void _DoCreateConfig_Click()
        {
            if (!_statusExcel)
            {
                F_ConfigDisease wConfig = new F_ConfigDisease(this);
                wConfig.ShowDialog();
            }
        }

        public ICommand bCreateHelp_Click => new RelayCommand(_DoCreateHelp_Click, _AlwaysTrue);
        private void _DoCreateHelp_Click()
        {
            F_Welcome wHelp = new F_Welcome();
            wHelp.ShowDialog();
        }

        public ICommand bCreateSimulation_Click => new RelayCommand(_DoCreateSimulation_Click, _AlwaysTrue);
        private void _DoCreateSimulation_Click()
        {
            CurSimulation.SetNewSession(
                        SizeWidth, SizeHeight,
                        AmountZdNothing, AmountZdMask, AmountZdSocDist, AmountZdAll,
                        AmountIncNothing, AmountIncMask, AmountIncSocDist, AmountIncAll,
                        AmountProdNothing, AmountProdMask, AmountProdSocDist, AmountProdAll,
                        AmountClinNothing, AmountClinMask, AmountClinSocDist, AmountClinAll,
                        AmountVzdNothing, AmountVzdMask, AmountVzdSocDist, AmountVzdAll,
                        AmountAsymptNothing, AmountAsymptMask, AmountAsymptSocDist, AmountAsymptAll,
                        StatusCollision);
            _curNode = CurSimulation.Root;
            ClearMap();
            CreateAndAddFigures(CurSimulation.People);

            _regionBackground.Width = CurSimulation.SizeWidth;
            _regionBackground.Height = CurSimulation.SizeHeight;
            _lineHMP.Y1 = CurSimulation.SizeHeight / 2;
            _lineHMP.X2 = CurSimulation.SizeWidth;
            _lineHMP.Y2 = CurSimulation.SizeHeight / 2;

            _lineVMP.X1 = CurSimulation.SizeWidth / 2;
            _lineVMP.X2 = CurSimulation.SizeWidth / 2;
            _lineVMP.Y2 = CurSimulation.SizeHeight;

            WindowHeight = 750;
            WindowWidth = 1920 / 2;

            _scaleNode.ScaleX = Math.Min(WindowWidth / CurSimulation.SizeWidth, WindowHeight / CurSimulation.SizeHeight);
            _scaleNode.ScaleY = Math.Min(WindowWidth / CurSimulation.SizeWidth, WindowHeight / CurSimulation.SizeHeight);

            _timer.Stop();
            _statusTimer = false;
            BIStartStopSim = _biStart;
            _statusDebug = false;
            BISocDist = _biSocDistVisible;
            CurSimulation.OnOffCollision(StatusCollision);

            if (_statusExcel && _excelApp.Visible)
            {
                CreateWorkbook();
                _curLine = 2;
                WriteLineExcel();
                WriteConfigExcel();
            }
        }

        public ICommand bClearFields_Click => new RelayCommand(_DoClearFields_Click, _AlwaysTrue);
        private void _DoClearFields_Click()
        {
            SizeWidth = 10; SizeHeight = 10;
            AmountZdNothing = 0; AmountZdMask = 0; AmountZdSocDist = 0; AmountZdAll = 0;
            AmountIncNothing = 0; AmountIncMask = 0; AmountIncSocDist = 0; AmountIncAll = 0;
            AmountProdNothing = 0; AmountProdMask = 0; AmountProdSocDist = 0; AmountProdAll = 0;
            AmountClinNothing = 0; AmountClinMask = 0; AmountClinSocDist = 0; AmountClinAll = 0;
            AmountVzdNothing = 0; AmountVzdMask = 0; AmountVzdSocDist = 0; AmountVzdAll = 0;
            AmountAsymptNothing = 0; AmountAsymptMask = 0; AmountAsymptSocDist = 0; AmountAsymptAll = 0;
            OnPropertyChanged(nameof(SizeWidth)); OnPropertyChanged(nameof(SizeWidth));
            OnPropertyChanged(nameof(AmountZdNothing)); OnPropertyChanged(nameof(SizeWidth));
            OnPropertyChanged(nameof(AmountZdSocDist)); OnPropertyChanged(nameof(AmountZdAll));
            OnPropertyChanged(nameof(AmountIncNothing)); OnPropertyChanged(nameof(AmountIncMask));
            OnPropertyChanged(nameof(AmountIncSocDist)); OnPropertyChanged(nameof(AmountIncAll));
            OnPropertyChanged(nameof(AmountProdNothing)); OnPropertyChanged(nameof(AmountProdMask));
            OnPropertyChanged(nameof(AmountProdSocDist)); OnPropertyChanged(nameof(AmountProdAll));
            OnPropertyChanged(nameof(AmountClinNothing)); OnPropertyChanged(nameof(AmountClinMask));
            OnPropertyChanged(nameof(AmountClinSocDist)); OnPropertyChanged(nameof(AmountClinAll));
            OnPropertyChanged(nameof(AmountVzdNothing)); OnPropertyChanged(nameof(AmountVzdMask));
            OnPropertyChanged(nameof(AmountVzdSocDist)); OnPropertyChanged(nameof(AmountVzdAll));
            OnPropertyChanged(nameof(AmountAsymptNothing)); OnPropertyChanged(nameof(AmountAsymptMask));
            OnPropertyChanged(nameof(AmountAsymptSocDist)); OnPropertyChanged(nameof(AmountAsymptAll));
        }

        public ICommand bCreateExcel_Click => new RelayCommand(_DoCreateExcel_Click, _AlwaysTrue);
        private void _DoCreateExcel_Click()
        {
            if (!_statusExcel)
            {
                try
                {
                    _excelApp = new Excel.Application { Visible = true };
                    CreateWorkbook();
                    _statusExcel = true;
                    _curLine = 2;
                    WriteLineExcel();
                    WriteConfigExcel();
                    BIExcel = _biExcelOff;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {

                _curLine--;
                CreateDiagramm("A1", "H" + _curLine.ToString(), "Диаграмма эпид процесса", "График количества заболевших", 1);
                _excelApp.Quit();
                _statusExcel = false;
                BIExcel = _biExcelOn;
            }
        }

        private void CreateWorkbook()
        {
            _excelApp.Workbooks.Close();
            _excelApp.SheetsInNewWorkbook = 1; 
            Excel.Workbook workbook = _excelApp.Workbooks.Add(Type.Missing);
            workbook.Windows[1].Zoom = 77;
            workbook.Saved = true; 

            Excel.Sheets excelSheets = _excelApp.Workbooks.get_Item(1).Sheets;
            Excel.Worksheet workSheet = (Excel.Worksheet)excelSheets.get_Item(1);
            workSheet.Name = "Население";
            Excel.Range cells = (Excel.Range)workSheet.Cells[1, 1];
            cells.Value2 = "Итерация";
            cells = (Excel.Range)workSheet.Cells[1, 2];
            cells.Value2 = "Восприимчивые";
            cells = (Excel.Range)workSheet.Cells[1, 3];
            cells.Value2 = "Больные в инкубационном периоде";
            cells = (Excel.Range)workSheet.Cells[1, 4];
            cells.Value2 = "Больные в продромальном периоде";
            cells = (Excel.Range)workSheet.Cells[1, 5];
            cells.Value2 = "Больные в клиническом периоде";
            cells = (Excel.Range)workSheet.Cells[1, 6];
            cells.Value2 = "Бессимптомные больные";
            cells = (Excel.Range)workSheet.Cells[1, 7];
            cells.Value2 = "Выздоровевшие";
            cells = (Excel.Range)workSheet.Cells[1, 8];
            cells.Value2 = "Летальные исходы";
            cells = (Excel.Range)workSheet.Cells[1, 9];
            cells.Value2 = "Среднее количество заражений";

            cells = (Excel.Range)workSheet.get_Range("A1", "I1");
            cells.HorizontalAlignment = Excel.Constants.xlCenter;
            cells.VerticalAlignment = Excel.Constants.xlCenter;
            cells.ColumnWidth = 20;
            cells.RowHeight = 45;
            cells.WrapText = true;
            cells.Borders.ColorIndex = 1;
            cells.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
            cells.Borders.Weight = Excel.XlBorderWeight.xlThin;

            cells = (Excel.Range)workSheet.get_Range("A2", "I50");
            cells.Borders.ColorIndex = 1;
            cells.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
            cells.Borders.Weight = Excel.XlBorderWeight.xlThin;
        }

        private void WriteLineExcel()
        {
            if (_statusExcel && _excelApp.Visible)
            {
                Excel.Sheets excelSheets = _excelApp.Workbooks.get_Item(1).Sheets;
                Excel.Worksheet workSheet = (Excel.Worksheet)excelSheets.get_Item(1);

                Excel.Range cells = (Excel.Range)workSheet.Cells[_curLine, 1];
                cells.Value2 = CurSimulation.Iter;
                cells = (Excel.Range)workSheet.Cells[_curLine, 2];
                cells.Value2 = CurSimulation.AmountZd;
                cells = (Excel.Range)workSheet.Cells[_curLine, 3];
                cells.Value2 = CurSimulation.AmountInc;
                cells = (Excel.Range)workSheet.Cells[_curLine, 4];
                cells.Value2 = CurSimulation.AmountProdorm;
                cells = (Excel.Range)workSheet.Cells[_curLine, 5];
                cells.Value2 = CurSimulation.AmountClin;
                cells = (Excel.Range)workSheet.Cells[_curLine, 6];
                cells.Value2 = CurSimulation.AmountAsympt;
                cells = (Excel.Range)workSheet.Cells[_curLine, 7];
                cells.Value2 = CurSimulation.AmountVzd;
                cells = (Excel.Range)workSheet.Cells[_curLine, 8];
                cells.Value2 = CurSimulation.AmountDied;
                cells = (Excel.Range)workSheet.Cells[_curLine, 9];
                cells.Value2 = CurSimulation.StR0;

                _curLine++;
            }
        }

        private void WriteConfigExcel()
        {
            if (_statusExcel && _excelApp.Visible)
            {
                Excel.Sheets excelSheets = _excelApp.Workbooks.get_Item(1).Sheets;
                Excel.Worksheet workSheet = (Excel.Worksheet)excelSheets.get_Item(1);

                Excel.Range cells = (Excel.Range)workSheet.get_Range("K2","S32");
                cells.Merge(Type.Missing);
                cells.Borders.ColorIndex = 1;
                cells.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                cells.Borders.Weight = Excel.XlBorderWeight.xlThick;
                cells.HorizontalAlignment = Excel.Constants.xlCenter;
                cells.VerticalAlignment = Excel.Constants.xlCenter;
                cells.Value2 = "Параметры модели" +
                    "\n\nХарактеристики заболевания" +
                    "\nПродолжительность инкубационного периоде: от " + Config.TimeIncub_A + " до " + Config.TimeIncub_B + " итерации" +
                    "\nПродолжительность продромального периода: от " + Config.TimeProdorm_A + " до " + Config.TimeProdorm_B + " итерации" +
                    "\nПродолжительность клинического периода: от " + Config.TimeRecovery_A + " до " + Config.TimeRecovery_B + " итерации" +
                    "\nЛетальность заболевания: " + Config.ProbabilityDie*100 + " %" +
                    "\nЧастота бессимптомных: " + Config.ProbabilityAsymptomatic*100 + " %" +
                    "\n\nУровень взаимодействия" +
                    "\nРадиус человека: " + Config.RadiusHuman +
                    "\nДальность возможных рукопожатий: " + (Config.RadiusContact - Config.RadiusHuman) +
                    "\nДальность возможных встреч: " + (Config.RadiusAirborne - Config.RadiusHuman) +
                    "\nРадиус социальной дистанции: " + (Config.RadiusSocDist - Config.RadiusHuman) +
                    "\n\nУровень распространения: " +
                    "\nБазовый шанс заразиться воздушно-капельным путем: " + Config.ProbabilityInfAirborne*100 + " %" +
                    "\nБазовый шанс заразиться контактным путем: " + Config.ProbabilityInfContact*100 + " %" +
                    "\nЭффективность защиты маски заразить кого-то: " + Config.MaskProtectionFrom*100 + " %" +
                    "\nЭффективность защиты маски заразиться от кого-то " + Config.MaskProtectionFor*100 + " %" +
                    "\n\nУровень заболеваемости" +
                    "\nВремя до следующей возможной встречи: от " + Config.TimeAirborne_A + " до " + Config.TimeAirborne_B + " итерации" +
                    "\nВремя до следующего возможного рукопожатия: от " + Config.TimeContact_A + " до " + Config.TimeContact_B + " итерации" +
                    "\nВремя до следующего мытья рук: от " + Config.TimeWash_A + " до " + Config.TimeWash_B + " итерации" +
                    "\nВремя до следующего контакта рук с лицом: от " + Config.TimeHandToFaceContact_A + " до " + Config.TimeHandToFaceContact_B + " итерации" +
                    "\nВремя до следующего загрязнения рук инфекцией: от " + Config.TimeInfHand_A + " до " + Config.TimeInfHand_B + " итерации";

            }
        }

        private void CreateDiagramm(string coord1, string coord2, string listName, string diagrName, int numSheet)
        {
            if (_statusExcel && _excelApp.Visible)
            {
                Excel.Sheets excelSheets = _excelApp.Workbooks.get_Item(1).Sheets;
                Excel.Worksheet workSheet = (Excel.Worksheet)excelSheets.get_Item(numSheet);
                Excel.Range cells = (Excel.Range)workSheet.get_Range(coord1, coord2);
                cells.Select();
                Excel.Chart chart = (Excel.Chart)_excelApp.Charts.Add(Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                chart.Activate();
                chart.Select(Type.Missing);
                _excelApp.ActiveChart.ChartType = Excel.XlChartType.xlXYScatterLinesNoMarkers;
                _excelApp.ActiveChart.HasTitle = true;
                _excelApp.ActiveChart.ChartTitle.Text = diagrName;
                chart.Name = listName;
                chart.SetSourceData(cells, Type.Missing);
            }
        }

        


       





    }
}
