/*call hap.livetiles.RegisterTileHandler
This takes 2 parameters, 1 is either a regular expression of the type of tile, or the full name of the tile, for short tile type names, you may need to use a regular expression e.g. /^me/gi which equals type starts with me
The 2nd parameter is a function that takes 3 parameters (technically 2)
You then need to create html id to use, I've used initdata.Group + initdata.Name with whitespaces and non alphanumberical characters removed
You then create the standard html a link, you can customize this generation if you want, but it needs to be an <a href> tag, not <div> or <span>
You then append that to the group that the tile will belong to, this is done by using $("#" + initdata.Group).append(somehtml usually this.html);
To make the tile live a timeout or interval is needed to kick start AJAX calls

For callbacks, you will need to define a seperate functoin after RegisterTileHandler is called, this will take what every you pass to it as it's entirely your domain, most cases you'll need this.id to reference the html id of the tile
*/
hap.livetiles.RegisterTileHandler("example", function (type, initdata, size) {
    this.id = (initdata.Group + initdata.Name).replace(/[\s'\/\\\&\.\,\*]*/gi, "");
    this.html = '<a id="' + this.id + '" href="' + hap.common.resolveUrl(initdata.Url) + '" target="' + initdata.Target + '" title="' + initdata.Description + '"' + (size == 'large' ? ' class="large"' : '') + (initdata.Color == '' ? '' : ' style="background-color: ' + initdata.Color.Base + ';" onmouseover="this.style.backgroundColor = \'' + initdata.Color.Light + '\';" onmouseout="this.style.backgroundColor = \'' + initdata.Color.Base + '\';" onmousedown="this.style.backgroundColor = \'' + initdata.Color.Dark + '\';"') + '><span><i style="background-image: url(' + hap.common.resolveUrl(initdata.Icon) + ');"></i><label></label></span>' + initdata.Name + '</a>';
    $("#" + initdata.Group).append(this.html);
    setInterval("mylivetilecallback('" + this.id + "');", 10000);
});
function mylivetilecallback(id) {
}