- PlayerSpeedChanger

VRChatワールドでローカルプレイヤーの移動速度、ジャンプ、重力を
ワールドに配置したUIからリアルタイムに変更できます。
UDONSharp で作成しています。

- Requirements

Unity 2022.3.22f1
VRChatSDK World 3.10.5
TextMesh Pro 3.0.6

- Installation

VCCに以下のリポジトリを追加してください。
https://lubin851.github.io/PlayerSpeedChanger/index.json

- Usage

1. プロジェクトへ「Player Speed Changer」を追加して開きます。
2. Project タブの Package > PlayerSpeedChanger にある、
プレハブの「PlayerSpeedChanger」と名前が付いたものをシーンに配置してください。
また、横移動と歩きを同値として扱うプレハブは名前に「WalkStrafeMatch」が付いています。
3. 動作確認して完了です。
4. 値を制限する際には直接スライダーの上限と下限を設定してください。

- API

外部から呼び出すことで働くメソッド一覧
　※WalkとStrafe統合モードではStrafeをWalkに強制同値にする。
 - 各停止
LockWalk
LockRun
LockStrafe
LockJump
LockGravity

- 各停止の解除
UnlockWalk
UnlockRun
UnlockStrafe
UnlockJump
UnlockGravity

- License

MIT license
Copyright (c) 2026 尾黒こう (Ryuban)

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and 
associated documentation files (the “Software”), to deal in the Software without 
restriction, including without limitation the rights to use, copy, modify, merge, publish, 
distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the 
Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND 
NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, 
DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.