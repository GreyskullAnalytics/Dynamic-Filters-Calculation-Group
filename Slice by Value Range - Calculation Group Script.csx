// by Johnny Winter
// www.greyskullanalytics.com

//script creates a calculation group that allows you to slice measures by a value range
//change the next 2 string variables for different naming conventions

//add the name of your calculation group here
string calcGroupName = "@Measure Filter Value";

//add the name for the column you want to appear in the calculation group
string columnName = "Filter Value";

//set the upper limit for the ranges you would like to slice by. This list should be comma seperated. 
//So if you want to slice measures by less than 1000, less than 100 and less than 10, you should enter: 1000, 100, 10
int[] rangeValues = {   
    
1000, 100, 10

};
// ----- do not modify script below this line -----
  
//check to see if a table with this name already exists
//if it doesnt exist, create a calculation group with this name
if (!Model.Tables.Contains(calcGroupName)) {
  var cg = Model.AddCalculationGroup(calcGroupName);
  cg.Description = "Contains value ranges than can be used to filter measures.";
};
//set variable for the calc group
Table calcGroup = Model.Tables[calcGroupName];

//if table already exists, make sure it is a Calculation Group type
if (calcGroup.SourceType.ToString() != "CalculationGroup") {
  Error("Table exists in Model but is not a Calculation Group. Rename the existing table or choose an alternative name for your Calculation Group.");
  return;
};
//by default the calc group has a column called Name. If this column is still called Name change this in line with specfied variable
if (calcGroup.Columns.Contains("Name")) {
  calcGroup.Columns["Name"].Name = columnName;
};


//create calculation items based on selected measures, including check to make sure calculation item doesnt exist
foreach(var cg in Model.CalculationGroups) {
    if (cg.Name == calcGroupName) {
        int calcOrdinal = 0;
        foreach(var i in rangeValues) {
            calcOrdinal = calcOrdinal + 1;
            var calcItemName = "<= " + String.Format("{0:n0}", i);
            if (!cg.CalculationItems.Contains(calcItemName)) {
                var newCalcItem = cg.AddCalculationItem(
                calcItemName, "IF (SELECTEDMEASURE() <= " + i + ", SELECTEDMEASURE())");
                    newCalcItem.FormatStringExpression = "SELECTEDMEASUREFORMATSTRING()";
                    newCalcItem.Ordinal = calcOrdinal;
            };
        };
    };
};

