# Player Speed Changer

これはVRChat ワールド用の自分自身の移動速度、ジャンプ、重力をリアルタイムに変更できるUI付きギミックです。<br>
個々のローカルで動作します。

APIには一時的に任意のステータス（例えばジャンプなど）を無効化があります。

CSharp 、uGUI、UDON 等の学習の一環で作成しました。

## AI利用の表明

仕様に対してどうのようなコードや構成をするのかの情報提供と教え役として利用しています。<br>
実際には提示された内容に対しては、自身による検索と提案で対抗しながら理解をしていく内容です。

コードは触りや基礎を生成してもらいつつも、コピペはせずに手打ちと修正と反証を提示し返して作成しています。<br>
仕様とデザインは自作据え置きです。

## 導入手順
1. [こちら](vcc://vpm/addRepo?url=https%3A%2F%2Flubin851.github.io%2FPlayerSpeedChanger%2Findex.json) をクリックすると VCC にコミュニティリポジトリに登録されます。
1. プロジェクトへ「Player Speed Changer」を追加して開きます。
1. Project タブの Package > PlayerSpeedChanger > Runtime にある、<br>
プレハブの「PlayerSpeedChanger」と名前が付いたものをシーンに配置してください。<br>
また、横移動と歩きを同値として扱うプレハブは名前に「WalkStrafeMatch」が付いています。
1. 動作確認して完了です。
2. 値を制限する際には直接スライダーの上限と下限を設定してください。


# VPM と VCC 対応
このリポジトリの本懐は、VCC へリポジトリ登録する方法を学ぶ事です。<br>
まだコードが読めず雰囲気で個人でなんとなくGitを触っているレベルが基準です。

完遂するために必要な技術は以下の通りです。
* GitHubを扱い、Gitへクローンやプッシュができる
* GitHub上またはリポジトリ内のデータや設定を編集する
* Unityのアセットのみならず、エクスプローラー（ディレクトリという表現でも正しい？）から直接データを扱う
* 公式のドキュメントをよく読み、リポジトリの README をよく読む
* テンプレート内のデータもざっくり読めること
* 逆ドメインなどの専門的な用法を理解すること
  
**それでも**エラーに対して見当が付かないので、（AIに頼るのが不愉快で不服ながら）折れて Codex に聞いて見てもらいながらミスを修正しました。<br>

## > ドキュメントとテンプレート
公式ドキュメントには **一連の導入とテンプレートがあります。**<br>
個人的にはVPM よりも、 **先に GitHub へログインして[テンプレートを複製](https://vcc.docs.vrchat.com/guides/create-listing)するほうがスムーズです。**<br>
ページの「Use this template」を押すことで、GitHub上に **複製した新規リポジトリ** を **作成** できます。

コピー元の **公式テンプレートは[こちら](https://github.com/vrchat-community/template-package-listing)。**<br>
手順ではクローンしたプロジェクトを開いて VPM による Package も一緒に行っています。

### 引用
* GitHub リポジトリのリストの公開までの手順[https://github.com/vrchat-community/template-package-listing]
* VPM の使い方[https://vcc.docs.vrchat.com/guides/convert-unitypackage]
* テンプレート。READMEに大体の事は書いてある。[https://github.com/vrchat-community/template-package-listing]


### 問題に引っかかったメモ

#### > VCCでいざインポートすると、コンパイルエラーになる

コンパイルエラーの内容は UDON や USharp や TextMeshPro が参照されない事でした。
原因としては Assets にあった時はプロジェクト全体から自動的に参照していたのですが、<br>
Package に移動したことでそれがされないようです。<br>
（using UDON などが赤波々になっていて、宣言してもダメという事）

その為、 **明示的にどれを使うか参照をはっきりさせる必要があります。**

* .asmdef に使用する他 .asmdef を参照する

これは Assembly Definition と呼ばれるもので、利用したい他の Assembly Defintion を登録することで明示的に定義できます。<br>
VPM でコンバートした後、 パッケージ名フォルダ > Runtime にあるパッケージ名のアセットです。<br>
Unity上では拡張子が見えないので、エクスプローラで確認してください。

これを選択し、インスペクターで「Assembly Definition References」のリストに登録します。<br>
使う事が確定の SDK は登録されていますが、USharp、TextMeshPro、uGUI が今回不足しました。<br>
リストを追加して一覧に出ますが、上記に必要な .asmdef は以下を使いました。
1.  UdonSharpRuntime
2.  Unity.TextMeshPro
3.  UnityEngine.UI

