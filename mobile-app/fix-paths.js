const fs = require('fs');
const path = require('path');

const distDir = path.join(__dirname, 'dist');
const htmlPath = path.join(distDir, 'index.html');
const unityHtmlPath = path.join(distDir, 'unity', 'index.html');
const nojekyllPath = path.join(distDir, '.nojekyll');
const html404Path = path.join(distDir, '404.html');
const public404Path = path.join(__dirname, 'public', '404.html');

// Fix main React Native app index.html
let html = fs.readFileSync(htmlPath, 'utf8');

// Add base tag for GitHub Pages subdirectory
html = html.replace(
  '<meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no" />',
  '<meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no" />\n    <base href="/cardGameCollection/">'
);

// Add SPA redirect script for GitHub Pages to handle reloads
const spaRedirectScript = `
    <script type="text/javascript">
      // GitHub Pages SPA redirect handler
      (function(l) {
        if (l.search[1] === '/' ) {
          var decoded = l.search.slice(1).split('&').map(function(s) {
            return s.replace(/~and~/g, '&')
          }).join('?');
          window.history.replaceState(null, null,
              l.pathname.slice(0, -1) + decoded + l.hash
          );
        }
      }(window.location))
    </script>`;

html = html.replace('</head>', spaRedirectScript + '\n  </head>');

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

// Copy 404.html for GitHub Pages SPA support
if (fs.existsSync(public404Path)) {
  fs.copyFileSync(public404Path, html404Path);
  console.log('✅ Copied 404.html for GitHub Pages SPA support');
}

// Flatten nested assets directory structure
// Expo places assets at dist/assets/src/assets/* but we need them at dist/assets/*
const nestedAssetsPath = path.join(distDir, 'assets', 'src', 'assets');
if (fs.existsSync(nestedAssetsPath)) {
  const targetAssetsPath = path.join(distDir, 'assets');

  // Function to recursively copy files
  function copyRecursive(src, dest) {
    const items = fs.readdirSync(src);

    for (const item of items) {
      const srcPath = path.join(src, item);
      const destPath = path.join(dest, item);

      if (fs.statSync(srcPath).isDirectory()) {
        if (!fs.existsSync(destPath)) {
          fs.mkdirSync(destPath, { recursive: true });
        }
        copyRecursive(srcPath, destPath);
      } else {
        fs.copyFileSync(srcPath, destPath);
      }
    }
  }

  // Copy all files from nested location to root assets
  copyRecursive(nestedAssetsPath, targetAssetsPath);
  console.log('✅ Flattened nested assets directory');
}

// Fix asset paths in JavaScript bundles
// Replace 'assets/src/assets/' with 'assets/' and fix absolute paths
const jsDir = path.join(distDir, '_expo', 'static', 'js', 'web');
if (fs.existsSync(jsDir)) {
  const jsFiles = fs.readdirSync(jsDir).filter(file => file.endsWith('.js'));

  jsFiles.forEach(file => {
    const filePath = path.join(jsDir, file);
    let content = fs.readFileSync(filePath, 'utf8');
    const originalContent = content;

    // Replace nested asset paths with flattened paths
    content = content.replace(/assets\/src\/assets\//g, 'assets/');

    // Fix absolute asset URIs - remove leading slash so they respect base tag
    // Change uri:"/assets/ to uri:"assets/
    content = content.replace(/uri:"\/assets\//g, 'uri:"assets/');

    if (content !== originalContent) {
      fs.writeFileSync(filePath, content);
      console.log(`✅ Fixed asset paths in ${file}`);
    }
  });
}

console.log('✅ Fixed paths in index.html');
console.log('✅ Fixed paths in unity/index.html');
console.log('✅ Created .nojekyll file');
console.log('✅ Created .gitignore to force include Build folder');
