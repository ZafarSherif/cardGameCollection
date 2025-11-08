const { getDefaultConfig } = require('expo/metro-config');

const config = getDefaultConfig(__dirname);

// Add publicPath support for GitHub Pages subdirectory
if (process.env.PUBLIC_URL) {
  config.transformer = {
    ...config.transformer,
    publicPath: process.env.PUBLIC_URL,
  };
}

module.exports = config;
