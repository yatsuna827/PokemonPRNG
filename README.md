# PokemonPRNG

ポケモンの乱数処理に使用される各種PRNG（疑似乱数生成器）アルゴリズムのC#実装を提供するライブラリコレクションです。

## パッケージ一覧

### [PokemonPRNG.LCG32.Core](./PokemonPRNG.LCG32/)
- 32bit LCGの共通インタフェースを提供するモジュールです。

### [PokemonPRNG.LCG32.StandardLCG](./PokemonPRNG.LCG32/)
- 第3世代および第4世代のメインRNGとして使われているパラメータの32bit LCGの実装を提供するライブラリです。

### [PokemonPRNG.LCG32.GCLCG](./PokemonPRNG.LCG32/)
- ポケモンコロシアムおよびポケモンXDのメインRNGとして使われているパラメータの32bit LCGの実装を提供するライブラリです。

### [PokemonPRNG.MT](./PokemonPRNG.MT/)
- 第4世代の一部（ID決定や孵化など）および第5世代（個体値乱数列）、第6世代で使われているメルセンヌツイスタの実装を提供するライブラリです。

### [PokemonPRNG.LCG64](./PokemonPRNG.LCG64/)
- 第5世代で使われているパラメータの64bit LCGの実装を提供するライブラリです。

### [PokemonPRNG.TinyMT](./PokemonPRNG.TinyMT/)
- 第6世代・第7世代で使われているTinyMTの実装を提供するライブラリです。

### [PokemonPRNG.SFMT](./PokemonPRNG.SFMT/), [PokemonPRNG.SFMT.SIMD](./PokemonPRNG.SFMT.SIMD/)
- 第7世代で使われているSFMTの実装を提供するライブラリです。SFMT.SIMDはSIMD命令に対応した実装になっています。

### [PokemonPRNG.Xoroshiro128p](./PokemonPRNG.Xoroshiro128p/)
- 第8世代で使われているXoroshiro128+の実装を提供するライブラリです。
- BDSP用の実装も提供します。

### [PokemonPRNG.XorShift128](./PokemonPRNG.XorShift128/)
- BDSPで使われているXorshift128の実装を提供するライブラリです。

### [PokemonPRNG.LCG32.StaticLCG](./PokemonPRNG.LCG32/), [PokemonPRNG.LCG32.AlternativeLCG](./PokemonPRNG.LCG32/)
- おまけです。

## 開発環境

### ビルド
Visual Studioでソリューション(`PokemonPRNG.slnx`)をビルド：
- **ソリューション全体**: Ctrl+Shift+B
- **特定プロジェクト**: 右クリック → ビルド
- **リリース構成**: 構成マネージャーでRelease選択

### テスト実行
Visual Studio Test Explorerを使用：
- **テストエクスプローラー**: テスト → テストエクスプローラー
- **全テスト実行**: Ctrl+R, A

### ベンチマーク
BenchmarkプロジェクトをRelease構成で実行

## システム要件

- .NET Standard 2.0以降（ほぼ全パッケージ）
- .NET 7.0以降（SFMT.SIMDのみ）
- Visual Studio 2019以降 推奨

## ライセンス

[MIT License](./LICENSE)

## Author

[夜綱](https://twitter.com/sub_827)

---

各パッケージの詳細な使用方法は、それぞれのREADMEファイルを参照してください。
