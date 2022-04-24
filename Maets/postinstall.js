const fse = require('fs-extra');
const path = require('path');

const nodeModulesDir = path.join(__dirname, 'node_modules');
const publicDir = path.join(__dirname, 'wwwroot/public');

fse.emptyDirSync(path.join(publicDir, 'jquery'));
fse.emptyDirSync(path.join(publicDir, 'tinymce'));
fse.emptyDirSync(path.join(publicDir, 'tinymce-jquery'));
fse.emptyDirSync(path.join(publicDir, 'chart.js'));
fse.copySync(path.join(nodeModulesDir, 'jquery', 'dist'), path.join(publicDir, 'jquery'), { overwrite: true });
fse.copySync(path.join(nodeModulesDir, 'tinymce'), path.join(publicDir, 'tinymce'), { overwrite: true });
fse.copySync(path.join(nodeModulesDir, '@tinymce', 'tinymce-jquery', 'dist'), path.join(publicDir, 'tinymce-jquery'), { overwrite: true });
fse.copySync(path.join(nodeModulesDir, 'chart.js', 'dist'), path.join(publicDir, 'chart.js'), { overwrite: true });
