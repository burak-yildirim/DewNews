// Note this only includes basic configuration for development mode.
// For a more comprehensive configuration check:
// https://github.com/fable-compiler/webpack-config-template

const path = require("path");
const HtmlWebpackPlugin = require("html-webpack-plugin");
const CopyWebpackPlugin = require("copy-webpack-plugin");
const MiniCssExtractPlugin = require("mini-css-extract-plugin");
const webpack = require("webpack");

const CONFIG = {
  indexHtmlTemplate: "./src/index.html",
  fableEntry: "./src/js/App.js",
  outputDir: "./public",
  assetsDir: "./src/public",
  devServerPort: 8080
};

function absPath(filePath) {
  return path.isAbsolute(filePath) ? filePath : path.join(__dirname, filePath);
}

// If we're running the webpack-dev-server, assume we're in development mode
const isProduction = !process.argv.find(v => v.indexOf("webpack-dev-server") !== -1);
const environment = isProduction ? "production" : "development";
process.env.NODE_ENV = environment;
console.log("Bundling for " + environment + "...");

let commonPlugins = [
  new HtmlWebpackPlugin({
    filename: "index.html",
    template: absPath(CONFIG.indexHtmlTemplate)
  })
];
commonPlugins = isProduction ? commonPlugins.concat([
  new MiniCssExtractPlugin({ filename: "style.[name].css" }), // "style.[name].[hash].css"
  new CopyWebpackPlugin({ patterns: [{ from: absPath(CONFIG.assetsDir) }] })
])
: commonPlugins.concat([
  new webpack.HotModuleReplacementPlugin()
]);

module.exports = {
  mode: environment,
  entry: {
    app: absPath(CONFIG.fableEntry)
  },
  output: {
    path: absPath(CONFIG.outputDir),
    filename: "[name].js" // isProduction ?  "[name].[hash].js" : "[name].js"
  },
  devtool: isProduction ? "source-map" : "eval-source-map",
  optimization: {
    splitChunks: {
      chunks: "all"
    }
  },
  devServer: {
    publicPath: "/",
    contentBase: absPath(CONFIG.assetsDir), // "./public",
    port: CONFIG.devServerPort,
    hot: true,
    inline: true
  },
  resolve: {
    symlinks: false
  },
  module: {
    rules: [
      // css config from postcss & tailwindcss docs
      {
        test: /\.css/,
        exclude: /node_modules/,
        use: [
          // in production MiniCssExtractPlugin can be used here.
          // It seperates css files per user js files.
          "style-loader",
          {
            loader: "css-loader",
            options: {
              importLoaders: 1
            }
          },
          "postcss-loader"
        ]
      },
      {
        test: /\.(ico|png|jpg|jpeg|gif|svg|woff|woff2|ttf|eot)(\?.*)?$/,
        use: [ "file-loader" ]
      },
      {
        test: /\.js$/,
        enforce: "pre",
        use: [ "source-map-loader" ]
      }
    ]
  },
  plugins: commonPlugins
}
