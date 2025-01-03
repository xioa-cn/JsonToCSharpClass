# JsonToCSharpClass

## 简介
`JsonToCSharpClass` 是一个 WPF 应用程序，用于将 JSON 数据结构转换成 C# 类。该工具支持复杂的嵌套 JSON，并提供了一些可配置的选项，如生成虚拟属性等。

## 功能
- **JSON 解析**：将输入的 JSON 字符串解析成 C# 类。
- **自定义命名空间**：用户可以指定生成的类的命名空间。
- **生成虚拟属性**：可选生成虚拟属性，以便在派生类中重写。
- **复制到剪贴板**：一键复制生成的 C# 代码到剪贴板。
- **保存到文件**：将生成的 C# 代码保存到本地文件。
- **关于页面**：提供应用程序的详细信息和开发者联系方式。

## 开始使用
### 系统要求
- Windows 10 或更高版本
- .NET 6.0 或更高版本

### 安装
目前，`JsonToCSharpClass` 需要从源代码编译：
1. 克隆仓库到本地：
   ```bash
       git clone https://github.com/xioa-cn/JsonToCSharpClass.git
   ```
2. 使用 Visual Studio 2022 或更高版本打开解决方案文件。
3. 构建解决方案并运行。

### 使用方法
1. 启动应用程序。
2. 在主界面输入 JSON 字符串。
3. 点击“转换”按钮，下方将显示生成的 C# 类代码。
4. 使用“复制到剪贴板”或“保存到文件”功能按需操作生成的代码。

## 贡献
欢迎通过 Pull Requests 或 Issues 来提交功能请求或报告 bug。请确保您的代码符合项目的编码标准。

## 许可证
该项目使用 MIT 许可证，详情请见 `LICENSE` 文件。

## 联系方式
- Email: xliu552@163.com
- GitHub: [https://github.com/developer](https://github.com/xioa-cn/JsonToCSharpClass)]
