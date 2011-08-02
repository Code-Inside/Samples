function createElements()
{
    var count = document.getElementById('Count').value;
    
    for(var i = 0; i < count; i++)
        {
        var itemDiv = document.createElement('div');
        itemDiv.id = 'element_' + i;
        itemDiv.className = 'Elements';
        
        var silverlightDiv = document.createElement('div');
        silverlightDiv.id = 'element_silverlight_' + i;
        silverlightDiv.className = 'Silverlights';
        itemDiv.appendChild(silverlightDiv);
        createSilverlight(silverlightDiv, i);
        
        document.getElementById('AppHost').appendChild(itemDiv);
        }
}

function createSilverlight(parentElement, id)
{
    var slPluginId = 'SilverlightPlugIn_' + id;
	Silverlight.createObjectEx({
		source: 'Template.xaml',
		parentElement: parentElement,
		id: slPluginId,
		properties: {
			width: '295',
			height: '189',
			background:'#ffffffff',
            isWindowless: 'false',
			version: '1.0'
		},
		events: {
			onError: null,
			onLoad: onComplete
		},		
		context: id 
	});

}

function onComplete(sender, usercontext, eventArgs)
{
   sender.content.FindName("TitleText").Text = "My Item: " + usercontext;
   sender.content.FindName("TitleShadow").Text = "My Item: " + usercontext;
}

if (!window.Silverlight) 
	window.Silverlight = {};

Silverlight.createDelegate = function(instance, method) {
	return function() {
        return method.apply(instance, arguments);
    }
}