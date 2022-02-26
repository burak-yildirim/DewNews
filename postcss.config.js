module.exports = {
  plugins: [
    require("postcss-import"),
    require("tailwindcss/nesting"),
    require("tailwindcss"),
    require("autoprefixer")
    // require("postcss-nested")
  ]
  // plugins: {
  //   tailwindcss: {},
  //   autoprefixer: {},
  //   "postcss-nested": {}
  // }
}
