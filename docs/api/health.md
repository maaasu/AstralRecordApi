# 疎通確認 API

## 概要

外部システムから API サーバーへ接続できるかを確認するための疎通確認用エンドポイントです。

## エンドポイント

| 項目 | 値 |
|---|---|
| メソッド | GET |
| パス | `/api/health` |

## リクエスト

パラメータなし。

```http
GET /api/health
```

## レスポンス

### 200 OK

API サーバーへの接続成功。

```json
{
	"status": "ok",
	"service": "AstralRecordApi",
	"timestamp": "2026-03-20T10:00:00+00:00"
}
```

| フィールド | 型 | 説明 |
|---|---|---|
| `status` | string | 常に `"ok"` |
| `service` | string | サービス名 |
| `timestamp` | string (ISO 8601) | レスポンス生成時刻 (UTC) |
