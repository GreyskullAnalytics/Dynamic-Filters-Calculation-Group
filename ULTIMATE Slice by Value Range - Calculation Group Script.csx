// by Johnny Winter
// www.greyskullanalytics.com

//script creates a calculation group that allows you to slice measures by a value range 
//and a calculation group to control whihc measure this is applied to
//change the next 5 string variables for different naming conventions

//add the name of your calculation group for selecting the filter value here
string calcGroupName_filterValue = "@Measure Filter Value";

//add the name for the column you want to appear in the calculation group
string columnName_filterValue = "Filter Value";

//add the name of your calculation group for selectiong which measure to filter by here
string calcGroupName_filterSelection = "@Measure Filter Selection";

//add the name for the column you want to appear in the calculation group
string columnName_filterSelection = "Filter Selection";

//add the name of the filter selection measure here
string measureName = "Filter Selection Measure";

//set the upper limit for the ranges you would like to slice by. This list should be comma seperated. 
//So if you want to slice measures by less than 1000, less than 100 and less than 10, you should enter: 1000, 100, 10
int[] rangeValues = {   
    
1000, 100, 10

};
// ----- do not modify script below this line -----

if (Selected.Measures.Count == 0) {
  Error("Select one or more measures");
  return;
}

//create calc group to control which measure to filter by

//check to see if a table with this name already exists
//if it doesnt exist, create a calculation group with this name
if (!Model.Tables.Contains(calcGroupName_filterSelection)) {
  var cg = Model.AddCalculationGroup(calcGroupName_filterSelection);
  cg.Description = "Contains a column called " + columnName_filterSelection + " which allows you to select which measure is used by the " + calcGroupName_filterValue + " feature. The " + measureName + " measure allows the filtering to be applied to your visual.";
};
//set variable for the calc group
Table calcGroup = Model.Tables[calcGroupName_filterSelection];

//if table already exists, make sure it is a Calculation Group type
if (calcGroup.SourceType.ToString() != "CalculationGroup") {
  Error("Table exists in Model but is not a Calculation Group. Rename the existing table or choose an alternative name for your Calculation Group.");
  return;
};
//by default the calc group has a column called Name. If this column is still called Name change this in line with specfied variable
if (calcGroup.Columns.Contains("Name")) {
  calcGroup.Columns["Name"].Name = columnName_filterSelection;
};

//check to see if filter selection measure has been created, if not create it now
//if a measure with that name alredy exists elsewhere in the model, throw an error
if (!calcGroup.Measures.Contains(measureName)) {
  foreach(var m in Model.AllMeasures) {
    if (m.Name == measureName) {
      Error("This measure name already exists in table " + m.Table.Name + ". Either rename the existing measure or choose a different name for the measure in your Calculation Group.");
      return;
    };
  };
  var newMeasure = calcGroup.AddMeasure(
  measureName, "0");
};

//create calculation items based on selected measures, including check to make sure calculation item doesnt exist
foreach(var cg in Model.CalculationGroups) {
  if (cg.Name == calcGroupName_filterSelection) {
    foreach(var m in Selected.Measures) {
      if (!cg.CalculationItems.Contains(m.Name)) {
        var newCalcItem = cg.AddCalculationItem(
        m.Name, "IF ( " + "ISSELECTEDMEASURE ( [" + measureName + "] ), " + "[" + m.Name + "], " + "SELECTEDMEASURE() )");
        newCalcItem.FormatStringExpression = "IF ( " + "ISSELECTEDMEASURE ( [" + measureName + "] ),\"" + m.FormatString + "\", SELECTEDMEASUREFORMATSTRING() )";
      };
    };
  };
};


//create calc group for selecting the value to filter by

//check to see if a table with this name already exists
//if it doesnt exist, create a calculation group with this name
if (!Model.Tables.Contains(calcGroupName_filterValue)) {
  var cg = Model.AddCalculationGroup(calcGroupName_filterValue);
  cg.Description = "Contains value ranges than can be used to filter measures.";
};
//set variable for the calc group
calcGroup = Model.Tables[calcGroupName_filterValue];

//if table already exists, make sure it is a Calculation Group type
if (calcGroup.SourceType.ToString() != "CalculationGroup") {
  Error("Table exists in Model but is not a Calculation Group. Rename the existing table or choose an alternative name for your Calculation Group.");
  return;
};

//precedence for this calc group must be set to 1 and it needs to execute after the measure selection calc group
(calcGroup as CalculationGroupTable).CalculationGroup.Precedence = 1;

//by default the calc group has a column called Name. If this column is still called Name change this in line with specfied variable
if (calcGroup.Columns.Contains("Name")) {
  calcGroup.Columns["Name"].Name = columnName_filterValue;
};


//create calculation items based on selected measures, including check to make sure calculation item doesnt exist
foreach(var cg in Model.CalculationGroups) {
    if (cg.Name == calcGroupName_filterValue) {
        int calcOrdinal = 0;
        foreach(var i in rangeValues) {
            calcOrdinal = calcOrdinal + 1;
            var calcItemName = "<= " + String.Format("{0:n0}", i);
            if (!cg.CalculationItems.Contains(calcItemName)) {
                var newCalcItem = cg.AddCalculationItem(
                calcItemName, "IF ( [" + measureName + "] <= " + i + ", SELECTEDMEASURE())");
                    newCalcItem.FormatStringExpression = "SELECTEDMEASUREFORMATSTRING()";
                    newCalcItem.Ordinal = calcOrdinal;
            };
        };
    };
};

