AnjLab.Tests = {};

AnjLab.Tests.UnitTestsRunner = AnjLab.Class.create({
    runUnitTests : function(fixtures, logNode){
        if (!logNode){
            var div = document.createElement("div");
            div.align = "center";
            logNode = document.createElement("table");
            div.appendChild(logNode );
            document.body.appendChild(div);
        }
        
        AnjLab.Tests.UnitTestsRunner.logNode = logNode;
        AnjLab.Tests.UnitTestsRunner.fixtures = fixtures;    
        AnjLab.Tests.UnitTestsRunner.fixtureIndex = -1;
        AnjLab.Tests.UnitTestsRunner.runNextFixture();
    },
    
    runNextFixture:function(){
        AnjLab.Tests.UnitTestsRunner.fixtureIndex++;
        if (!AnjLab.Tests.UnitTestsRunner.testsComplete())
        {
            var nextFixture = AnjLab.Tests.UnitTestsRunner.fixtures[AnjLab.Tests.UnitTestsRunner.fixtureIndex];
            nextFixture.runTests(AnjLab.Tests.UnitTestsRunner.logNode);
            // start check timeout
            var interval = setInterval(function(){
                if (nextFixture.allTestsComplete())
                {
                    clearInterval(interval);
                    AnjLab.Tests.UnitTestsRunner.runNextFixture();
                }    
            }, 100);
        }
    },
    
    testsComplete : function(){
        return (AnjLab.Tests.UnitTestsRunner.fixtures.length < AnjLab.Tests.UnitTestsRunner.fixtureIndex + 1); 
    },
    
    getErrorsReport:function(){
        var report = "";
        for (var i = 0; i < AnjLab.Tests.UnitTestsRunner.fixtures.length; i++){
            var fixture = AnjLab.Tests.UnitTestsRunner.fixtures[i];
            report += fixture.getErrorsReport();
        }
        return report;
    }
},
{});

AnjLab.Tests.TestFixture = AnjLab.Class.create({
    getTestErrorReport:function(test){
        return "in " + test.name + ": " + test.errors[0] + "<pre>" + test.method + "</pre>";
    }
},
{
    name : "AnjLab.Tests.TestFixture",
    
    setUp: function(){},
    tearDown : function(){},
    
    runTests:function(reportNode){
        this.incompleteTests = [];
        this.failedTests = [];
        for (member in this)
            if (member.startsWith("test") && typeof this[member] == "function")
                this.incompleteTests.push({name:member, method:this[member], errors:[]});
        
        if (AnjLab.Browser.isIE)
            this.incompleteTests.reverse();
        
        this.testsCount = this.incompleteTests.length;
        this.visualizer = new AnjLab.Tests.TestFixtureVisualizer(this, reportNode);
        this.setUp();
        Assert.onError = this.onAssertionError.bind(this);
        this.runNextTest();
    },
    
    runNextTest:function(){
        if (!this.allTestsComplete()){
            this.currentTest = this.incompleteTests[0];
            try{
                this.currentTest.method.call(this);
            }
            catch(e){
                this.onAssertionError("Exception in test: " + e.message);
                this.end();
            }
        }
        else
            this.tearDown(); 
    },
    
    onAssertionError:function(message){ 
        this.currentTest.failed = true;
        this.currentTest.errors.push(message);
    },
    
    end:function(){
        if (this.isCompletedTest(this.currentTest))
            return;
        
        if (this.currentTest.failed)
            this.failedTests.push(this.currentTest);
            
        this.incompleteTests.shift();
        this.visualizer.updateIndicator(this.currentTest);
        this.runNextTest();
    },
    
    allTestsComplete: function(){
        return this.incompleteTests.length == 0;
    },
    
    isCompletedTest: function(test){
        for (var i = 0; i < this.incompleteTests.length; i++)
            if (this.incompleteTests[i].name == test.name)
                return false;
        return true;                
    },
    
    getErrorsReport: function(){
        if (this.failedTests.length == 0)
            return "";
            
        var report = "Fixture " + this.name + " errors:<br/>";
        for (var i = 0; i < this.failedTests.length; i++)
            report += AnjLab.Tests.TestFixture.getTestErrorReport(this.failedTests[i]);
        return report;
    }
});

AnjLab.Tests.TestFixtureVisualizer = AnjLab.Class.create({},
{
    initialize: function(fixture, reportTable) {
        this.fixture = fixture;
        
        reportTable.width = "400px";
        reportTable.cellSpacing = 0;
        reportTable.cellPadding = 0;
        // header
        var headerTD = reportTable.insertRow(-1).insertCell(-1);
        headerTD.innerHTML = "<b>" + this.fixture.name + "(" + this.fixture.testsCount + ")" + "</b>";
        // indicator
        this.indicator = document.createElement("div");
        this.indicator.style.width = "0%";
        this.indicator.style.height = "20px"; 
        this.indicator.style.backgroundColor = "#00ff00";
        var indicatorTD = reportTable.insertRow(-1).insertCell(-1);
        indicatorTD.appendChild(this.indicator);
        indicatorTD.align = "left";
        // report
        this.report = reportTable.insertRow(-1).insertCell(-1);
        this.report.align = "left";
        this.report.style.fontSize = "11px";
        this.report.style.borderBottom = "1px solid black";
    },
    
    printReport: function(){
        this.insertInReport(this.fixture.getErrorsReport());
    },
    
    updateIndicator: function(justEndedTest){
        if (justEndedTest.failed){
            this.indicator.style.backgroundColor = "#ff0000";
            this.insertInReport("<span style='background:#ff0000'>&nbsp;&nbsp;</span>&nbsp;" + justEndedTest.name + " - failed");
            this.insertInReport(AnjLab.Tests.TestFixture.getTestErrorReport(justEndedTest));
        }
        else
            this.insertInReport("<span style='background:#00ff00'>&nbsp;&nbsp;</span>&nbsp;" + justEndedTest.name + " - success");
        
        var completePercent = 100 * (this.fixture.testsCount - this.fixture.incompleteTests.length) / this.fixture.testsCount;
        this.indicator.style.width = completePercent + "%";
    },
    
    insertInReport:function(html){
        var div = document.createElement("div");
        div.innerHTML = html;
        div.style.margin= "2px 0px";
        this.report.appendChild(div);
    }
});

Assert = AnjLab.Class.create({
    errors:[],
    currentTest:null,
    
    doAssert: function(expression, message) {
		message = message || 'Expected TRUE';
		if ((typeof expression == 'boolean') && (expression == false))
		{
		    Assert.errors.push(message);   
		    Assert.onError(message);
        }
	},
	
	onError:function(message){
	},
	
	isTrue: function(expression, message) {
		message = message || 'Expected true';
		Assert.doAssert(expression, message);
	},
	
	isFalse: function(expression, message) {
		message = message || 'Expected false';
		Assert.doAssert(!expression, message);
	},
	
	areEqual: function(a, b, message) {
		message = message || 'Expected ' + b + ' but was ' + a;
		Assert.doAssert((a == b), message);
	},
	
	areNotEqual: function(a, b, message) {
		message = message || 'Expected ' + a + ' not equals to ' + b;
		Assert.doAssert((a != b), message);
	},
	
	isNull: function(a, message) {
		message = message || 'Expected null but was ' + a;
		Assert.doAssert((a === null), message);
	},
	
	isNotNull: function(a, message) {
		message = message || 'Expected not null';
		Assert.doAssert((a !== null), message);
	}
},
{});

AnjLab.Tests.AssertionTests = AnjLab.Class.inherit(AnjLab.Tests.TestFixture, {},
{
    name : "AnjLab.Tests.AssertionTests",
    
    setUp: function(){
    },
    
    tearDown : function(){
    },
    
    testEquals: function(){ 
        Assert.areEqual(1, 1);
        Assert.areEqual(0, 0);
        Assert.areEqual(-1, -1);
        
        Assert.areEqual(true, true);
        Assert.areEqual(false, false);
        
        Assert.areEqual("str", "str");
        Assert.areEqual("", "");
        this.end();
    },
    
    testNotEquals : function(){
        Assert.areNotEqual(1, 2);
        Assert.areNotEqual(true, false);
        Assert.areNotEqual("str", "str1");
        
        Assert.areNotEqual(1, false);
        this.end();
    },
    
    testIsTrue:function(){
        Assert.isTrue(true);
        Assert.isTrue(1 == 1);
        Assert.isTrue("" == "");
        this.end();
    },
    
    testIsFalse:function(){
        Assert.isFalse(false);
        Assert.isFalse(1 == 2);
        Assert.isFalse("" == "str");
        this.end();
    },
    
    testIsNull:function(){
        Assert.isNull(null);
        this.end();
    },
    
    testIsNotNull:function(){
        Assert.isNotNull(0);
        Assert.isNotNull("");
        Assert.isNotNull(false);
        this.end();
    }
});