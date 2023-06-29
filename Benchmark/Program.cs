using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using PokemonPRNG.SFMT;
using PokemonPRNG.MT;

var switcher = new BenchmarkSwitcher(new[]
{
    typeof(SFMTBenchmark),
    typeof(MTBenchmark)
});

args = new string[] { "0" };
switcher.Run(args);


public class BenchmarkConfig : ManualConfig
{
    public BenchmarkConfig()
    {
        AddExporter(MarkdownExporter.GitHub); // ベンチマーク結果を書く時に出力させとくとベンリ
        AddDiagnoser(MemoryDiagnoser.Default);

        // ShortRunを使うとサクッと終わらせられる、デフォルトだと本気で長いので短めにしとく。
        // ShortRunは LaunchCount=1  TargetCount=3 WarmupCount = 3 のショートカット
        //AddJob(Job.ShortRun);
    }
}

[Config(typeof(BenchmarkConfig))]
public class SFMTBenchmark
{
    private readonly uint initialSeed = 0xBEEFFACE;

    private SFMT? sfmt;
    private CachedSFMT? cached;

    [IterationSetup]
    public void Setup()
    {
        sfmt = new SFMT(initialSeed);
        cached = new CachedSFMT(initialSeed, 3);
    }

    public IEnumerable<object[]> Loops()
    {
        yield return new object[] { 1000000, 100 };
        yield return new object[] { 20000, 5000 };
    }

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(Loops))]
    public long SFMT(int mainLoop, int innerLoop)
    {
        var sum = 0u;
        for (int i = 0; i < mainLoop; i++, sfmt.Advance())
        {
            var temp = sfmt!.Clone();
            for (int k = 0; k < innerLoop; k++)
                sum += temp.GetRand32();
        }

        return sum;
    }

    [Benchmark]
    [ArgumentsSource(nameof(Loops))]
    public long CachedSFMT(int mainLoop, int innerLoop)
    {
        var sum = 0u;
        for (int i = 0; i < mainLoop; i++, cached!.Advance())
        {
            for (int k = 0; k < innerLoop; k++)
                sum += cached!.GetRand32();
        }

        return sum;
    }
}

[Config(typeof(BenchmarkConfig))]
public class MTBenchmark
{
    private readonly uint initialSeed = 0xBEEFFACE;

    private MT? mt;
    private CachedMT? cached;

    [IterationSetup]
    public void Setup()
    {
        mt = new MT(initialSeed);
        cached = new CachedMT(initialSeed, 3);
    }

    public IEnumerable<object[]> Loops()
    {
        yield return new object[] { 1000000, 100 };
        yield return new object[] { 20000, 5000 };
    }

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(Loops))]
    public long MT(int mainLoop, int innerLoop)
    {
        var sum = 0u;
        for (int i = 0; i < mainLoop; i++, mt.Advance())
        {
            var temp = mt!.Clone();
            for (int k = 0; k < innerLoop; k++)
                sum += temp.GetRand();
        }

        return sum;
    }

    [Benchmark]
    [ArgumentsSource(nameof(Loops))]
    public long Cached(int mainLoop, int innerLoop)
    {
        var sum = 0u;
        for (int i = 0; i < mainLoop; i++, cached!.MoveNext())
        {
            for (int k = 0; k < innerLoop; k++)
                sum += cached!.GetRand();
        }

        return sum;
    }
}
