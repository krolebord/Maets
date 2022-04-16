const gulp = require('gulp');
const concat = require('gulp-concat');
const cleanCSS = require('gulp-clean-css');
const uglify = require('gulp-uglify-es').default;

const vendorStyles = [
    "node_modules/bootstrap-datepicker/dist/css/bootstrap-datepicker.css",
    "node_modules/select2/dist/css/select2.css",
    "wwwroot/css/bootstrap.css",
    "wwwroot/css/select2-bootstrap-5-theme.css"
];
const vendorScripts = [
    "node_modules/jquery/dist/jquery.js",
    "node_modules/jquery-validation/dist/jquery.validate.js",
    "node_modules/jquery-validation-unobtrusive/dist/jquery.validate.unobtrusive.js",
    "node_modules/popper.js/dist/umd/index.js",
    "node_modules/bootstrap/dist/js/bootstrap.js",
    "node_modules/bootstrap-datepicker/dist/js/bootstrap-datepicker.js",
    "node_modules/select2/dist/js/select2.full.js"
];

gulp.task('build-vendor-css', () => {
    return gulp.src(vendorStyles)
        .pipe(concat('vendor.min.css'))
        .pipe(cleanCSS())
        .pipe(gulp.dest('wwwroot/css'));
});

gulp.task('build-vendor-js', () => {
    return gulp.src(vendorScripts)
        .pipe(concat('vendor.min.js'))
        .pipe(uglify())
        .pipe(gulp.dest('wwwroot/js'));
});

gulp.task('build-vendor', gulp.parallel('build-vendor-css', 'build-vendor-js'));

gulp.task('default', gulp.series('build-vendor'));
