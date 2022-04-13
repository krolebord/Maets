const gulp = require('gulp');
const concat = require('gulp-concat');

const vendorStyles = [
    "node_modules/bootstrap-datepicker/dist/css/bootstrap-datepicker.css"
];
const vendorScripts = [
    "node_modules/jquery/dist/jquery.js",
    "node_modules/jquery-validation/dist/jquery.validate.js",
    "node_modules/jquery-validation-unobtrusive/dist/jquery.validate.unobtrusive.js",
    "node_modules/popper.js/dist/umd/index.js",
    "node_modules/bootstrap/dist/js/bootstrap.js",
    "node_modules/bootstrap-datepicker/dist/js/bootstrap-datepicker.js"
];

gulp.task('build-vendor-css', () => {
    return gulp.src(vendorStyles)
        .pipe(concat('vendor.min.css'))
        .pipe(gulp.dest('wwwroot/css'));
});

gulp.task('build-vendor-js', () => {
    return gulp.src(vendorScripts)
        .pipe(concat('vendor.min.js'))
        .pipe(gulp.dest('wwwroot/js'));
});

gulp.task('build-vendor', gulp.parallel('build-vendor-css', 'build-vendor-js'));

gulp.task('default', gulp.series('build-vendor'));
