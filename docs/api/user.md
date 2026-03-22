# ユーザー取得 API

## 概要

`dbo.user` テーブルから UUID をキーにユーザー情報を取得します。論理削除済みのユーザーは取得できません。

## エンドポイント

| 項目 | 値 |
|---|---|
| メソッド | GET |
| パス | `/api/user/{uuid}` |

## リクエスト

### パスパラメータ

| パラメータ | 型 | 必須 | 説明 |
|---|---|---|---|
| `uuid` | GUID | ✓ | Minecraft プレイヤーの UUID |

```http
GET /api/user/8d8d3d63-6c25-4bde-bf75-6ee3d3f26c46
```

## レスポンス

### 200 OK

ユーザー取得成功。

```json
{
	"uuid": "8d8d3d63-6c25-4bde-bf75-6ee3d3f26c46",
	"mcid": "PlayerName",
	"joinDate": "2026-03-09T12:34:56",
	"lastJoinDate": "2026-03-20T18:45:12",
	"globalIp": "127.0.0.1",
	"accountId": null,
	"banIndefinite": false,
	"banDate": null,
	"kickIp": true,
	"permission": 0,
	"createdAt": "2026-03-09T12:34:56",
	"updatedAt": "2026-03-20T18:45:12",
	"createdBy": "8d8d3d63-6c25-4bde-bf75-6ee3d3f26c46",
	"updatedBy": "8d8d3d63-6c25-4bde-bf75-6ee3d3f26c46",
	"isDeleted": false
}
```

| フィールド | 型 | 説明 |
|---|---|---|
| `uuid` | GUID | Minecraft プレイヤーの UUID |
| `mcid` | string | Minecraft ID |
| `joinDate` | string (ISO 8601) | 初回参加日時 |
| `lastJoinDate` | string (ISO 8601) | 最終参加日時 |
| `globalIp` | string | 最終ログイン IP アドレス |
| `accountId` | string \| null | 連携アカウント ID |
| `banIndefinite` | bool | 無期限 BAN フラグ |
| `banDate` | string \| null | BAN 解除日時 |
| `kickIp` | bool | IP キックフラグ |
| `permission` | int | 権限レベル |
| `createdAt` | string (ISO 8601) | レコード作成日時 |
| `updatedAt` | string (ISO 8601) | レコード更新日時 |
| `createdBy` | GUID | 作成者 UUID |
| `updatedBy` | GUID | 更新者 UUID |
| `isDeleted` | bool | 論理削除フラグ |

### 404 Not Found

指定 UUID のユーザーが存在しない、または論理削除済み。
