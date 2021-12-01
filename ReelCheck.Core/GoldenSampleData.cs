using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vrh.Redis.DataPoolHandler;

namespace ReelCheck.Core
{
    internal class GoldenSampleData
    {
        public GoldenSampleData(WorkStationType workStationType, ProcessType processType, InstanceWriter instanceWriter, string datatPool, string instanceName, string redisConnectionString)
        {
            _instanceWriter = instanceWriter;
            _workStationType = workStationType;
            _processType = processType;
            ReloadFromRedisCache(datatPool, instanceName, redisConnectionString);
        }

        /// <summary>
        /// Olvasott adatok
        /// </summary>
        public string ReadedData
        {
            get { lock (_locker) { return _readedData; } }
            set { lock (_locker) { _readedData = value; } }
        }
        private string _readedData = null;


        /// <summary>
        /// Esedékes a Goldensample futtatása
        /// </summary>
        public bool Due
        {
            get { lock (_locker) { return _due; } }
            set
            {
                lock (_locker)
                {
                    _due = value;
                    WriteToCache(DataPoolDefinition.GS.Due, value.ToString().ToUpper());
                }
            }
        }
        private bool _due = false;

        /// <summary>
        /// az esedékessé válás módja (TIMER(időzítés járt leÍ)/CYCLELIMIT(ciklus számláló korlátot ért el)/RERUN(hibás eredménnyel zárult teszt megismétlése)
        /// </summary>
        public GoldenSampleDueMode DueMode
        {
            get { lock (_locker) { return _dueMode; } }
            set
            {
                lock (_locker)
                {
                    _dueMode = value;
                    WriteToCache(DataPoolDefinition.GS.DueMode, value.ToString().ToUpper());
                }
            }
        }
        private GoldenSampleDueMode _dueMode = GoldenSampleDueMode.None;

        /// <summary>
        /// az esedékessé válás időbélyege
        /// </summary>
        public DateTime? DueTime
        {
            get { lock (_locker) { return _dueTime; } }
            set
            {
                lock (_locker)
                {
                    _dueTime = value;
                    WriteToCache(DataPoolDefinition.GS.DueTime, value.HasValue ? value.Value.ToString() : string.Empty);
                }
            }
        }
        private DateTime? _dueTime = null;

        /// <summary>
        /// Az utolsó goldensample teszt óta eltelt normál ciklusok száma
        /// </summary>
        public int CyclesFromLast
        {
            get { lock (_locker) { return _cyclesFromLast; } }
            set
            {
                lock (_locker)
                {
                    _cyclesFromLast = value;
                    WriteToCache(DataPoolDefinition.GS.CyclesFromLast, value.ToString());
                }
            }
        }
        private int _cyclesFromLast = 0;

        /// <summary>
        /// az aranyminta teszt indításának időbélyege
        /// </summary>
        public DateTime? StartTime
        {
            get { lock (_locker) { return _startTime; } }
            set
            {
                lock (_locker)
                {
                    _startTime = value;
                    WriteToCache(DataPoolDefinition.GS.StartTime, value.HasValue ? value.Value.ToString() : string.Empty);
                }
            }
        }
        private DateTime? _startTime = null;

        /// <summary>
        /// az aranyminta tesztben elvégzett ciklusok száma(sikeres teszt esetén azonos az érvényes limit-tel); aranyminta teszt végrehajtása alatt ennek a számlálónak az értéke folyamatosan aktualizálódik, tehát nem csak a rekord lezárásakor kerül végrehajtásra.
        /// </summary>
        public int GoldenSampleCycles
        {
            get { lock (_locker) { return _goldenSampleCycles; } }
            set
            {
                lock (_locker)
                {
                    _goldenSampleCycles = value;
                    WriteToCache(DataPoolDefinition.GS.GSCycles, value.ToString());
                }
            }
        }
        private int _goldenSampleCycles = 0;

        /// <summary>
        /// Az utolsó ismert goldensample teszt eredmény
        /// </summary>
        public GoldenSampleResult LastResult
        {
            get { lock (_locker) { return _lastResult; } }
            set
            {
                lock (_locker)
                {
                    _lastResult = value;
                    WriteToCache(DataPoolDefinition.GS.LastResult, value.ToString().ToUpper());
                }
            }
        }
        private GoldenSampleResult _lastResult = GoldenSampleResult.None;

        /// <summary>
        /// Az utoljára indított golden sample teszt indítási ideje
        /// </summary>
        public DateTime? LastStart
        {
            get { lock (_locker) { return _lastStart; } }
            set
            {
                lock (_locker)
                {
                    _lastStart = value;
                    WriteToCache(DataPoolDefinition.GS.LastStart, value.ToString());
                }
            }
        }
        private DateTime? _lastStart = null;

        /// <summary>
        /// Az utolsó sikeres goldensample teszt időpontja
        /// </summary>
        public DateTime? LastSuccessTest
        {
            get { lock (_locker) { return _lastSuccessTest; } }
            set
            {
                lock (_locker)
                {
                    _lastSuccessTest = value;
                    WriteToCache(DataPoolDefinition.GS.LastSuccessTest, value.ToString());
                }
            }
        }
        private DateTime? _lastSuccessTest = null;

        /// <summary>
        /// Active-e a golden sample mód
        /// </summary>
        public bool Active
        {
            get { lock (_locker) { return _active; } }
            set
            {
                lock (_locker)
                {
                    _active = value;
                    WriteToCache(DataPoolDefinition.GS.Active, value.ToString().ToUpper());
                }
            }
        }
        private bool _active = false;

        /// <summary>
        /// Kiiírja az adatok a Redis cache-re
        /// </summary>
        /// <typeparam name="T">adattípus</typeparam>
        /// <param name="key">redis kulcs</param>
        /// <param name="value">érték</param>
        private void WriteToCache<T>(DataPoolDefinition.GS key, T value)
        {
            try
            {
                if (typeof(T).IsEnum)
                {
                    _instanceWriter.WriteKeyValue<string>(key.GetRedisKey(), value.ToString().ToUpper());
                }
                else
                {
                    _instanceWriter.WriteKeyValue<T>(key.GetRedisKey(), value);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"{ex.Message}: {key}, {value}");
            }
        }

        /// <summary>
        /// Betölti a goldensample adatokata  redis cache-be mentett adatokból
        /// </summary>
        /// <param name="datatPool">az adatokat ez a pool tárolja</param>
        /// <param name="instanceName">az adatokat tartalmazó pool instance </param>
        /// <param name="redisConnectionString">a redis kiszolgáló falé nyitandó connection</param>
        private void ReloadFromRedisCache(string datatPool, string instanceName, string redisConnectionString)
        {
            using (InstanceReader reader = new InstanceReader(datatPool, instanceName, redisConnectionString, Serializers.XML))
            {
                LoadOrSetDefult<GoldenSampleResult>(DataPoolDefinition.GS.LastResult, LastResult, ref _lastResult, reader);
                LoadOrSetDefult<DateTime?>(DataPoolDefinition.GS.LastStart, LastStart, ref _lastStart, reader);
                LoadOrSetDefult<DateTime?>(DataPoolDefinition.GS.LastSuccessTest, LastSuccessTest, ref _lastSuccessTest, reader);
                LoadOrSetDefult<int>(DataPoolDefinition.GS.CyclesFromLast, CyclesFromLast, ref _cyclesFromLast, reader);
                LoadOrSetDefult<GoldenSampleDueMode>(DataPoolDefinition.GS.DueMode, DueMode, ref _dueMode, reader);
                LoadOrSetDefult<bool>(DataPoolDefinition.GS.Due, Due, ref _due, reader);
                LoadOrSetDefult<DateTime?>(DataPoolDefinition.GS.DueTime, DueTime, ref _dueTime, reader);
                LoadOrSetDefult<DateTime?>(DataPoolDefinition.GS.StartTime, StartTime, ref _startTime, reader);
                LoadOrSetDefult<int>(DataPoolDefinition.GS.GSCycles, GoldenSampleCycles, ref _goldenSampleCycles, reader);
                LoadOrSetDefult<bool>(DataPoolDefinition.GS.Active, Active, ref _active, reader);
            }
        }

        /// <summary>
        /// Betölti a redis cache-en lévő értéket, vagy a beállítja  adefult értéket
        /// </summary>
        /// <typeparam name="T">Típus</typeparam>
        /// <param name="key">redis kulcs</param>
        /// <param name="property">tulajdonság</param>
        /// <param name="field">field</param>
        /// <param name="reader">redis reader objektum</param>
        /// <param name="defaultValue">alapértelmezett érték</param>
        private void LoadOrSetDefult<T>(DataPoolDefinition.GS key, T property, ref T field, InstanceReader reader, T defaultValue = default(T))
        {
            var redisData = reader.GetCurrentData(DataPoolDefinition.GetRedisKey(key));
            if (redisData != null)
            {
                try
                {
                    var converter = TypeDescriptor.GetConverter(typeof(T));
                    if (converter != null)
                    {
                        field = (T)converter.ConvertFromString((string)redisData.Value);
                        return;
                    }
                }
                catch (NotSupportedException)
                {
                }
            }
            property = defaultValue;
        }

        /// <summary>
        /// Munkaállomás típusa
        /// </summary>
        private WorkStationType _workStationType;

        /// <summary>
        /// Munkafolymat típusa
        /// </summary>
        private ProcessType _processType;

        /// <summary>
        /// Redis DP instance writer
        /// </summary>
        private InstanceWriter _instanceWriter;

        /// <summary>
        /// Instance level locker
        /// </summary>
        private object _locker = new object();
    }

    /// <summary>
    /// A figyelmeztetés beállításának módja
    /// </summary>
    public enum GoldenSampleDueMode
    {
        /// <summary>
        /// Nincs figyelmeztetés
        /// </summary>
        None,
        /// <summary>
        /// Időzítés
        /// </summary>
        Timer,
        /// <summary>
        /// Ciklus limit
        /// </summary>
        CycleLimit,
        /// <summary>
        /// Kézi (beavatkozás)
        /// </summary>
        Iv,
    }

    /// <summary>
    /// Az aranyminta teszt eredménye
    /// </summary>
    public enum GoldenSampleResult
    {
        /// <summary>
        /// Nem történt/ismeretlen
        /// </summary>
        None,
        /// <summary>
        /// Jelenleg folyamatban
        /// </summary>
        InProgress,
        /// <summary>
        /// Sikeres
        /// </summary>
        Pass,
        /// <summary>
        /// Sikertelen
        /// </summary>
        Fail,
        /// <summary>
        /// Elutasított (törölték a határidő figyelmeztetést)
        /// </summary>
        Rejected,
    }
}
