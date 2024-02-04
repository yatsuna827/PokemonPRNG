# PokemonPRNG.LCG32

## 概要
32bit LCGを`uint`型の拡張メソッドや`IEnumerable<uint>`として扱えるようにするライブラリです.

## 使い方
ファイルの先頭で`using PokemonPRNG.LCG32.StandardLCG;`を宣言すると、
```cs
var seed = 0xC0FFEEu;
seed.Advance(827); // 827消費する.
for (int i=0; i<827; i++) {
    if (seed.GetRand(827) == 0) break; // 乱数を取得し, 827で割った余りが0なら終了.
}
```
のように書くことができます.

TODO: IEnumerableについて書く。

## 構成
### LCG32.XXX
namespaceによってLCGの種類が異なりますが、処理の内容は同じです.
| 名前 | 概要 | 更新処理 |
|:-|:-|:-|
| StandardLCG | 3,4世代で一般的に用いられるLCG. | `S_{n+1} = 0x41C64E6D * S_n + 0x6073` |
| GCLCG | コロシアム, XDで用いられるLCG. | `S_{n+1} = 0x343FD * S_n + 0x269EC3`|
| StaticLCG | IDくじ用の乱数等に用いられるLCG. | `S_{n+1} = 0x41C64E6D * S_n + 0x3039 `|

### 拡張メソッド

(もちろん普通に呼び出すこともできます).

| 名前 | 戻り値 | 引数 | 説明 |
|:-|:-|:-|:-|
| NextSeed | `uint` | `this uint` | 1個先のseedを取得.  |
| NextSeed | `uint` | `this uint, uint` | n個先のseedを取得. |
| PrevSeed | `uint` | `this uint` | 1個前のseedを取得.  |
| PrevSeed | `uint` | `this uint, uint` | n個前のseedを取得. |
| Advance | `uint` | `ref this uint` | seedを1つ進める.  |
| Advance | `uint` | `ref this uint, uint` | seedをn進める. |
| Back | `uint` | `ref this uint` | seedを1つ戻す.  |
| Back | `uint` | `ref this uint, uint` | seedをn戻す. |
| GetRand | `uint` | `ref this uint` | seedを1つ進め, 16bitの乱数値を取得.  |
| GetRand | `uint` | `ref this uint, uint` | seedを1つ進め, 剰余で乱数値を取得. |
| GetIndex | `uint` | `this uint` | 0x0からの消費数を取得.  |
| GetIndex | `uint` | `this uint, uint` | 指定した初期seedからの消費数を取得. |
