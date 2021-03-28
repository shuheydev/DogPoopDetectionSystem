# DogPoopDetectionSystem
ケージの中の犬用トイレを定期的に撮影して、💩を検出したら通知をする仕組みを作っています。

- Azure Storage(Blob)
- Custom Vision
- Azure Functions
- Xamarin.Forms

## 設定
/DogPoopDetectionSystem/Configurationsの中のBlobConfiguration.csとCustomVisionConfiguration.csの各プロパティに接続文字列などの必要な情報をセットすればOk。

## Xamarin.Forms
Xamarin.FormsアプリはクロスプラットフォームなのにAndroidアプリだけです。

iPhoneの開発環境がないのです。

Xamarin.FormsはCommunityToolKitでCameraViewが提供されるようになったので、カメラ機能をコントロールとしてアプリ内で使えるようになったのはとてもありがたいですね。

Timerで定期的に撮影してBlobに保存するだけのアプリです。

## Blob
Xamarin.Formsから写真がアップロードされて保存されます。

写真が保存されると、それをトリガーとしてFunction Appが実行されます。

## Function
Blobに保存された画像に対する物体検出処理をCustom VisionにSDK経由でリクエストを出します。

検出結果を受け取ったら通知を出す予定。

## Custom Vision
飼い犬の💩の画像を日々撮影して学習させた💩識別機。
