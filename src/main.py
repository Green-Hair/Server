# 导入Flask类库
from flask import Flask

# 创建应用实例
app = Flask(__name__)

# 视图函数（路由）
@app.route('/')
def index():
    return 'Hello World'

@app.route('/post/<str:email>')
def send_email(email: str):
    return 'nothing'

if __name__ == '__main__':
    app.run("127.0.0.1", 5418)
