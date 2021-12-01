using System;
using Vrh.Redis.DataPoolHandler;
using VRH.Log4Pro.MultiLanguageManager;

namespace ReelCheck.Core
{
    internal class GoldenSample
    {
        public GoldenSample(ReelCheck main, WorkStationType workStationType, ProcessType processType, InstanceWriter instanceWriter)
        {
            _main = main;            
            GoldenSampleData = new GoldenSampleData(workStationType, processType, instanceWriter, _main.PluginLevelConfiguration.DataPoolName, _main.Configuration.WorkStationType.ToString().ToUpper(), _main.PluginLevelConfiguration.RedisConnectionString);
        }

        /// <summary>
        /// Aranyminta teszt indítása
        /// </summary>
        public void Start()
        {
            if (_main.Configuration.GoldenSampeCycles < 1)
            {
                throw new Exception(MultiLanguageManager.GetWordCodeFromClassType(typeof(TrueWordCodes.ReelCheckService.UiMessage.Interventions.GoldenSample.Enter.Off)));
            }
            if (GoldenSampleData.Active)
            {
                throw new Exception(MultiLanguageManager.GetWordCodeFromClassType(typeof(TrueWordCodes.ReelCheckService.UiMessage.Interventions.GoldenSample.Enter.AlreadyRun)));
            }
            bool noEmpty = false;
            for (int i = 0; i < (_main.WorkstationData.WorkStationType == WorkStationType.Automatic ? 3 : 1); i++)
            {                
                switch (i)
                {
                    case 0:
                        noEmpty = !_main.Reels[ReelPosition.Identification].Empty;
                        break;
                    case 1:
                        noEmpty = !_main.Reels[ReelPosition.Print].Empty;
                        break;
                    case 2:
                        noEmpty = !_main.Reels[ReelPosition.Check].Empty;
                        break;
                }
                if (noEmpty)
                {                    
                    throw new Exception(MultiLanguageManager.GetWordCodeFromClassType(typeof(TrueWordCodes.ReelCheckService.UiMessage.Interventions.GoldenSample.Enter.NotEmpty)));
                }
            }            
            _main.WorkstationData.WriteToCache(DataPoolDefinition.WS.ReelCheckMode, ReelCheckMode.GoldenSample);
            GoldenSampleData.Active = true;
            GoldenSampleData.GoldenSampleCycles = 0;
            GoldenSampleData.StartTime = DateTime.Now;
        }

        /// <summary>
        /// Aranyminta teszt leállítása
        /// </summary>
        public void Stop(bool throwExceptions = true)
        {
            if (!GoldenSampleData.Active)
            {
                if (throwExceptions)
                {
                    throw new Exception(MultiLanguageManager.GetWordCodeFromClassType(typeof(TrueWordCodes.ReelCheckService.UiMessage.Interventions.GoldenSample.Exit.NotRun)));
                }
            }
            _main.WorkstationData.WriteToCache(DataPoolDefinition.WS.ReelCheckMode, ReelCheckMode.Normal);
            //GoldenSampleData.GoldenSampleCycles = 0;
            GoldenSampleData.Active = false;
            GoldenSampleData.StartTime = null;
            GoldenSampleData.ReadedData = String.Empty;
        }

        /// <summary>
        /// Törli az aranyminta teszt szükésgességére való figyelmeztetést
        /// </summary>
        public void DeleteDue(bool throwExceptions = true)
        {
            if (!GoldenSampleData.Due)
            {
                if (throwExceptions)
                {
                    throw new Exception(MultiLanguageManager.GetWordCodeFromClassType(typeof(TrueWordCodes.ReelCheckService.UiMessage.Interventions.GoldenSample.DeleteDue.NotDue)));
                }
            }
            if (!GoldenSampleData.Active)
            {
                GoldenSampleData.LastResult = GoldenSampleResult.Rejected;
                GoldenSampleData.LastSuccessTest = DateTime.Now;
                GoldenSampleData.LastStart = DateTime.Now;
                GoldenSampleData.CyclesFromLast = 0;
            }
            GoldenSampleData.Due = false;
            GoldenSampleData.DueMode = GoldenSampleDueMode.None;
            GoldenSampleData.DueTime = null;
        }

        /// <summary>
        /// Beállítja az aranyminta teszt szükségességére való figyelmeztetést
        /// </summary>
        public void SetDue(GoldenSampleDueMode dueMode, DateTime? dueDate = null, bool throwExceptions = true)
        {
            if (GoldenSampleData.Due)
            {
                if (throwExceptions)
                {
                    throw new Exception(MultiLanguageManager.GetWordCodeFromClassType(typeof(TrueWordCodes.ReelCheckService.UiMessage.Interventions.GoldenSample.SetDueNow.AlreadyDue)));
                }
            }
            GoldenSampleData.Due = true;
            GoldenSampleData.DueMode = dueMode;
            GoldenSampleData.DueTime = dueDate.HasValue ? dueDate : DateTime.Now;
            CheckDue();
        }

        /// <summary>
        /// Megvizsgálja a leolvasott adatot, ha golden sample módba van az aranyminta teszt szempontjából
        /// </summary>
        /// <param name="readedData"></param>
        /// <param name="reel"></param>
        public void GoldenSampleLabelRead(string readedData, ReelData reel)
        {
            if (_main.GoldenSampleModule.GoldenSampleData.Active && !readedData.Contains("EMPTY"))
            {
                reel.GoldenSample = true;
                reel.Empty = false;
                GoldenSampleData.GoldenSampleCycles++;
                if (String.IsNullOrEmpty(GoldenSampleData.ReadedData) && !readedData.ToUpper().Contains(SpecialLabelType.NoRead.ToString().ToUpper()))
                {
                    GoldenSampleData.ReadedData = readedData;
                }
                else
                {                    
                    if (GoldenSampleData.ReadedData == readedData && !readedData.ToUpper().Contains(SpecialLabelType.NoRead.ToString().ToUpper()))
                    {
                        reel.IdentificationReadResult = IdentificationResult.Pass;                        
                        if (_main.GoldenSampleModule.GoldenSampleData.GoldenSampleCycles >= _main.Configuration.GoldenSampeCycles)
                        {
                            // Pass
                            GoldenSampleData.LastResult = GoldenSampleResult.Pass;
                            GoldenSampleData.LastStart = GoldenSampleData.StartTime;
                            GoldenSampleData.CyclesFromLast = 0;
                            GoldenSampleData.LastSuccessTest = DateTime.Now;
                            DeleteDue(false);
                            Stop(false);
                            if (_main.WorkstationData.OperationBlocked 
                                && (_main.WorkstationData.BlockReasonCode == BlockReason.GoldenSampleTestDue 
                                || _main.WorkstationData.BlockReasonCode == BlockReason.GoldenSampleTestFailed))
                            {
                                _main.Unblock();
                            }
                        }
                    }
                    else
                    {
                        if (readedData.ToUpper().Contains(SpecialLabelType.NoRead.ToString().ToUpper()))
                        {
                            reel.IdentificationReadResult = IdentificationResult.NoRead;
                        }
                        else
                        {
                            reel.IdentificationReadResult = IdentificationResult.DataErr;
                        }
                        reel.ErrorMessage = MultiLanguageManager.GetTranslation(typeof(TrueWordCodes.ReelCheckService.UiMessage.Blocking.Reasons.GoldenSampleTestFailed));
                        // Fail
                        GoldenSampleData.LastResult = GoldenSampleResult.Fail;
                        GoldenSampleData.LastStart = GoldenSampleData.StartTime;
                        Stop(false);
                        if (!_main.WorkstationData.OperationBlocked)
                        {
                            _main.Block(BlockReason.GoldenSampleTestFailed);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Ciklus történik
        /// </summary>
        public void Cycle()
        {
            if (_main.Configuration.GoldenSampeCycles > 0 && !GoldenSampleData.Active)
            {
                GoldenSampleData.CyclesFromLast++;
                CheckDue();
            }
        }

        /// <summary>
        /// Ellenőrzi, hogy aranynóminta teszt határidő van-e
        /// </summary>
        public void CheckDue()
        {
            if (!GoldenSampleData.Due)
            {
                if (_main.Configuration.DueCycleCounterLimit > 0 && _main.Configuration.DueCycleCounterLimit < GoldenSampleData.CyclesFromLast)
                {
                    GoldenSampleData.Due = true;
                    GoldenSampleData.DueMode = GoldenSampleDueMode.CycleLimit;
                    GoldenSampleData.DueTime = DateTime.Now;
                    GoldenSampleData.GoldenSampleCycles = 0;
                }
                TimeSpan last = new TimeSpan(0, 0, 0);
                if (GoldenSampleData.LastSuccessTest.HasValue && GoldenSampleData.LastSuccessTest.Value.Date == DateTime.Now.Date)
                {
                    last = GoldenSampleData.LastSuccessTest.Value.TimeOfDay;
                }
                foreach (TimeSpan timer in _main.Configuration.DueTimers)
                {
                    if (timer > last && timer < DateTime.Now.TimeOfDay)
                    {
                        GoldenSampleData.Due = true;
                        GoldenSampleData.DueMode = GoldenSampleDueMode.Timer;
                        GoldenSampleData.DueTime = DateTime.Now.Date.Add(timer);
                        GoldenSampleData.GoldenSampleCycles = 0;
                        break;
                    }
                }
            }
            if (GoldenSampleData.Due)
            {
                if (_main.Configuration.GoldenSampleTestDueOperationBlockingEnabled && !_main.WorkstationData.OperationBlocked)
                {
                    _main.Block(BlockReason.GoldenSampleTestDue);
                }
            }
        }

        /// <summary>
        /// Aranyminta teszt adatok
        /// </summary>
        public GoldenSampleData GoldenSampleData { get; private set; }

        /// <summary>
        /// ReelCheck referencia a fő objektumok elérésére
        /// </summary>
        private ReelCheck _main;
    }
}
