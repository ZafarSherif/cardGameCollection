const fs = require('fs');
const path = require('path');

const distDir = path.join(__dirname, 'dist');
const htmlPath = path.join(distDir, 'index.html');
const unityHtmlPath = path.join(distDir, 'unity', 'index.html');
const nojekyllPath = path.join(distDir, '.nojekyll');

// Fix main React Native app index.html
let html = fs.readFileSync(htmlPath, 'utf8');

// Add base tag for GitHub Pages subdirectory
html = html.replace(
  '<meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no" />',
  '<meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no" />\n    <base href="/cardGameCollection/">'
);

// Convert absolute paths to relative paths so base tag works
// Fix favicon
html = html.replace('href="/favicon.ico"', 'href="favicon.ico"');

// Fix script paths (remove leading /)
html = html.replace(/src="\/_expo\//g, 'src="_expo/');

fs.writeFileSync(htmlPath, html);

// Fix Unity index.html
let unityHtml = fs.readFileSync(unityHtmlPath, 'utf8');

// Add base tag to Unity HTML so paths resolve correctly when in iframe
unityHtml = unityHtml.replace(
  '<meta http-equiv="Content-Type" content="text/html; charset=utf-8">',
  '<meta http-equiv="Content-Type" content="text/html; charset=utf-8">\n    <base href="/cardGameCollection/unity/">'
);

fs.writeFileSync(unityHtmlPath, unityHtml);

// Create .nojekyll file to prevent GitHub Pages from ignoring _expo folder
fs.writeFileSync(nojekyllPath, '');

// Create .gitignore in dist to ensure Build folder is included
const gitignorePath = path.join(distDir, '.gitignore');
fs.writeFileSync(gitignorePath, '# Include everything\n!*\n');

console.log('✅ Fixed paths in index.html');
console.log('✅ Fixed paths in unity/index.html');
console.log('✅ Created .nojekyll file');
console.log('✅ Created .gitignore to force include Build folder');
