## UsersController
### Create new user
http request: **POST**
```http request
http://localhost:5000/api/users
```
request body:
```json
{
    "nickname": "Werfi98",
    "password": "qwerty",
    "balance": 4000
}
```

### Read existing user
http request: **GET**
```http request
http://localhost:5000/api/users/{id}
```

### Update existing user
http request: **PUT**
```http request
http://localhost:5000/api/users
```
request body:
```json
{
  "id": 6,
  "nickname": "Werfi98",
  "password": "qwerty",
  "balance": 4500
}
```

### Delete existing user
http request: **DELETE**
```http request
http://localhost:5000/api/users/{id}
```