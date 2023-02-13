# 导入Flask类库
from flask import Flask

# 创建应用实例
app = Flask(__name__)

# 视图函数（路由）
@app.route('/')
def index():
	return 'Hello World'

if __name__ == '__main__':
	app.run("127.0.0.1",5418)