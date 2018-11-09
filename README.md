# pdfMaker
text(md) to pdf  
  
using iTextSharp  
  
visual studio等でc#プロジェクトとしてビルドしてください  
必要に応じてプログラムは変えてください(特にフォントとか色とか)  
  
# 使い方
/filesディレクトリにpresen.txtを作成  or テキストファイルをドラッグアンドドロップ  
実行  
pdfが完成  
  
先頭に`# `でタイトル/見出し 
先頭に`## `でサブタイトル、
`##`で解除
`---`で空白ページの挿入  
`"""`の行で囲むことで１スライドに複数行書き込み  
先頭に`- `、で箇条書き(空行は挟めない)  
`[image:path/to/image]`で画像の挿入  
`[pdf:path/to/pdf]`でpdfの埋め込み  
その他１行１スライド  
  
`(titleColor:color)`  
`(textColor:color)`  
で次からの文字の色を指定  

## 実装済みColor
BLACK  
RED  
GREEN  
BLUE  
CYAN  
MAGENTA  
YELLOW  

(16進colorCodeにも対応)

# その他
横に長いとはみ出ます  
画像のパスはlocalのファイルパスでもURLでも  
  
exeと同じディレクトリのconfigファイルの中に  
  
フォントのパス  
デフォルト入力ファイルパス  
デフォルト出力パス  
  
が書いてあります。変更可。


