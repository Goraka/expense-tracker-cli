# Expense Tracker CLI
A command line tool to track expenses built with .NET and utilizing System.CommandLine. A user friendly tool to track expense by categories and account with features to set monthly budget and balance accounts. 

## Features

1. Create Accounts : Users can create multiple accounts
1. Set Monthly Budget : Set a monthly against which all expenses will be balanced. Warnings will be given if budget exceeds.
1. Add Expense : Add Expenses with description by accounts
3. Delete Expense : Delete Expenses
4. List Expenses : List all saved expenses
5. Summarize Total : Summarizes total expended amount. Summarize by date range
6. Generate Report : Generate CSV report

## Setting Up

### 1. Clone Repository

``` 
git clone https://github.com/Goraka/expense-tracker-cli.git
cd expense-tracker
```
### 2. Build and Go

```
dotnet clean
dotnet build
```

### Usages

Options:
  -?, -h, --help  Show help and usage information
  --version       Show version information

Commands:
  create        Create a new account, budget or category
  add           Add an expense
  list, ls      List all expenses
  del, delete   Delete an expense
  sum, summary  Show a total summary of expenses and remaining budget

**To Create an Account or Set a new Budget**

```
expense-tracker create --account <"YOUR_ACCOUNT_NAME_HERE">

expense-tracker create --budget <NEW_BUDGET_AMOUNT_HERE>
``` 

**To Add a New Expense**

```
expense-tracker add -acc <ACCOUNT_NAME> --desc <"DESCRIPTION/CATEGORY"> --amt <EXPENSE_AMOUNT>

For Example: 
expense-tracker add -acc MyBank --desc "Lunch" --amt 20
```

**To List All Expense**

```
expense-tracker ls
```

**To Get a Summary of Expenses**

```
For total summary
expense-tracker sum 

For Periodic Summary
expense-tracker sum --range "03-03-2026" "10-03-2026" 
```
