# Astral Record API - Copilot Instructions

## プロジェクト概要

本プロジェクトは、Minecraft PurpurサーバのMMO RPGプラグイン **Astral Record** と、そのWebサイトに対してデータベース情報を送信するための **Web API** プロジェクトです。

## データベース構造

データは以下の2種類に分類して管理しています。

### 静的データ（マスタデータ）
- アイテム、クエストなどのマスタデータ
- YAMLなどのファイルで管理

### 動的データ（プレイヤー情報など）
- プレイヤーの進捗・状態など実行時に変化するデータ
- **SQL Server** で管理

## DB構造の詳細

DB構造の詳細定義は以下のGitHubプロジェクトのREADME.mdを参照してください。

- **プロジェクトURL**: https://github.com/maaasu/Database
- **参照ファイル**: README.md（基本情報セクション）

## 技術スタック

- **ランタイム**: .NET 10
- **フレームワーク**: ASP.NET Core Web API
- **データベース**: SQL Server

## ドキュメント運用

開発ルールの詳細は [README.md](../README.md) の「開発ルール」セクションを参照してください。

要約:
- API を追加・変更・削除した場合は `docs/api/*.md`、コントローラーの XML doc コメント、`README.md` の一覧テーブルをすべて更新すること
- Scalar API UI は `/scalar` で確認できる（サーバー起動後）
