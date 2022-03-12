module.exports = (api) => {
  return {
    plugins: [
      "postcss-import",
      "postcss-url",
      "tailwindcss/nesting",
      "tailwindcss",
      "autoprefixer"
    ]
  };
}
