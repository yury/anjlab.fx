
AnjLab.Dom = {
	getParent: function (element, tagName) {
	    while(element != null) {
			if(element.tagName == tagName) 
			    return element;
			element = element.parentNode;
		}
		return null;
	},
	
	getChildByTag:	function (element, tagName) {
		try {
			for(var i=0; i<element.childNodes.length; i++) {
				var child = element.childNodes.item(i);
				if(child.nodeType == 1) {
					if(child.tagName && child.tagName != tagName){
						if((childWithTag =  AnjLab.Dom.getChildByTag(child, tagName)) != null) 
						    return childWithTag;
					}
					else
					    return child;
				}
			}
		}
		catch(e){}
		return null;
	},
	
	getChildrenByClassName:	function (element, className) {
	    var result = new Array(); 
		try {
			for(var i=0; i<element.childNodes.length; i++) {
				var child = element.childNodes.item(i);
				if(child.nodeType == 1) {
					if(AnjLab.Dom.hasClassName(child, className)) 
						result.push(child);
					else 
					{
						if((childrenWithClassName =  AnjLab.Dom.getChildrenByClassName(child, className)).length > 0) 
							result = result.concat(childrenWithClassName);
					}
				}
			}
		}
		catch(e){}
		return result;
	},
	
	hasClassName: function(element, className) {
        if (!element)
          return;
        var a = element.className.split(' ');
        for (var i = 0; i < a.length; i++) {
          if (a[i] == className)
            return true;
        }
        return false;
    },
    
    getTableRowIndex:function(table, row){
        for (var i = 0; i < table.rows.length; i++)
            if (row == table.rows[i])
                return i;
        return -1;                
    },
    
    getValue:function(dom){
        switch (dom.tagName.toLowerCase())
        {
            case 'input':
                return dom.value;
            default:
                return dom.innerHTML;     
        }
        return null;
    },
    
    setValue:function(dom, value){
        switch (dom.tagName.toLowerCase())
        {
            case 'input':
                dom.value = value + "";
                break
            default:
                dom.innerHTML = value + "";     
        }
        return null;
    }
};