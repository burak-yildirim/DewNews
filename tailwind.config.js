module.exports = {
  // content: ["./src/**/*.{html,js}"],
  // content: [
  //   "./src/**/*.html",
  //   "./src/**/*.js"
  // ],
  purge: [
    "./src/**/*.html",
    "./src/**/*.js"
  ],
  darkMode: false,
  theme: {
    extend: {}
  },
  variants: {},
  plugins: [require("daisyui")]
}
