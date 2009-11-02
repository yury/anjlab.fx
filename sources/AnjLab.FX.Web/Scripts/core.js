/**
 * Basic inheritance based on prototype.js
 */
AnjLab.Class = {
	create: function(staticData, publicData) {
		var classElement = function() { 
			// For prevent empty calls in inheritance
			if(arguments[0] != Infinity && typeof this.initialize == 'function') {
				this.initialize.apply(this, arguments); 
			}
		};
		
		if(staticData)	Object.extend(classElement, staticData);
		if(publicData)	Object.extend(classElement.prototype, publicData);
		
		return classElement;
	},
	
	inherit: function(parentClass, staticData, publicData) {

		var classElement = function() { 
			// For prevent empty calls in inheritance
			if(arguments[0] != Infinity && typeof this.initialize == 'function') {
				this.initialize.apply(this, arguments); 
			}
		};
		
		classElement.$parents = [];
		
		if (parentClass) {
			var parentClasses = (parentClass instanceof Array)?(parentClass):([parentClass]);
			for(var i=0 , length=parentClasses.length; i<length; i++) {
				Object.extend(classElement.prototype, parentClasses[i].prototype); // Break instanceof relation :(
				classElement.$parents.push(parentClasses[i]);
			}
		}
		
		classElement.prototype.constructor = classElement;	
		
		if(staticData)	Object.extend(classElement, staticData);
		if(publicData)	Object.extend(classElement.prototype, publicData);
		
		return classElement;
	},
	
	
	singleton: function(parentClass, publicData) {
		var classElement = function() {};
		
		var parentClasses = (parentClass instanceof Array)?(parentClass):([parentClass]);
		for(var i=0; i<parentClasses.length; i++) {
			Object.extend(classElement.prototype, parentClasses[i].prototype); // Break instanceof relation :(
		}
		
		classElement.prototype.constructor = classElement;	
		
		var singleElement = new classElement(Infinity);
		if(publicData)	Object.extend(singleElement, publicData);
		
		singleElement.$parents = [];
		for(var i=0; i<parentClasses.length; i++) {
			singleElement.$parents.push(parentClasses[i]);
		}
		
		return singleElement;
	},
	
	base: function (classConstructor, classInstance, argv, fn) {
		var argv	= $A(argv);
		var fn		= fn || 'initialize';
		var result	= null;
		
		if(classConstructor.$parents && classConstructor.$parents.length) {
			for(var i=0; i<classConstructor.$parents.length; i++) {
				if(typeof classConstructor.$parents[i].prototype[fn] == 'function')
					result = classConstructor.$parents[i].prototype[fn].apply(classInstance, argv);
			}
		}
		
		return result;
	},
	
	instanceOf: function(objectName, objectType) {
		if(!objectName || !objectName.constructor) return false;
		if(objectName instanceof objectType) 
			return true
		else
			if(objectName.$parents) {
				// Singleton
				return AnjLab.Class.inheritedFrom(objectName, objectType);
			}
			else {
				// Class instance
				if(objectName.constructor === objectType) 
					return true;
				else 
					return AnjLab.Class.inheritedFrom(objectName.constructor, objectType);
			}
	},
	
	inheritedFrom: function(childClass, parentClass) {
		if(childClass.$parents instanceof Array) {
			for(var i=0; i<childClass.$parents.length; i++) {
				if(childClass.$parents[i] === parentClass || AnjLab.Class.inheritedFrom(childClass.$parents[i], parentClass)) return true;
			}
			return false;
		}
	},
	
	extendType: function(type, sdata, idata) {
	    if(sdata)	Object.extend(type, sdata);
		if(idata)	Object.extend(type.prototype, idata);
	}
};