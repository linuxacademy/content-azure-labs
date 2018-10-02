New-AzureRmResourceGroupDeployment -Name ExampleDeployment `
    -ResourceGroupName ReplaceMe `
    -TemplateFile template.json `
    -TemplateParameterFile parameters.json -Verbose
