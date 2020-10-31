@using static LMS.Extentions.OrderList
@model LMS.Models.ViewModels.CourseUnitViewModel
@{
    ViewData["Title"] = "UnitList";
    Layout = "~/Views/Shared/_Layout.cshtml";
    var itCounter = 0;

}

<br />
<br />

<div class="border backgroundWhite">

    <div class="row">
        <div class="col-8">
            <h2 class="text-info">@Model.Course.Name</h2>
            <br />
            <h4 class="text-info text-black-50">Assignments and Projects Per Unit</h4>
        </div>

    </div>

    <br />
    <div>
        @if (Model.CourseUnitList.Count() > 0)
        {
            <form>
                <table class="table table-striped border">

                    <tbody>


                        @foreach (var module in Model.CourseModuleList)
                        {
                        <thead>

                            <tr class="table-success table-sm">
                                <td colspan="7" cellpadding="2">
                                    Module: @Html.DisplayFor(m => module.Title)
                                </td>
                            </tr>

                        </thead>
                        @if (module.CourseUnitOrder != null && module.CourseUnitOrder != "")
                        {
                            @foreach (var unit in Model.CourseUnitList)
                            {

                                <tr class="table-striped table-primary">
                                    @if (ItemIdOrder(module.CourseUnitOrder).Contains(unit.Id))
                                    {
                                        itCounter++;
                                        <td>
                                            @{
                                                if (unit.Name.Length < 55)
                                                {
                                                    var result = itCounter.ToString() + ". " + "Unit: " + unit.Name;
                                                    @result;
                                                }
                                                else if (unit.Name.Length > 55)
                                                {
                                                    var unitTitle = itCounter.ToString() + ". " + "Unit: " + unit.Name.Substring(0, 55) + "....";
                                                    @unitTitle;
                                                }
                                            }

                                        </td>



                                    }
                                </tr>


                                @if (Model.UnitValidityPairs.Count() > 0)
                                {
                                    <thead class="table-striped">
                                        <tr>
                                            <th style="width:43%">Summative Title</th>
                                            <th style="text-align:center; width:13%">Type</th>
                                            <th style="text-align:center; width:13%">Open Date</th>
                                            <th style="text-align:center; width:15%">Close Date</th>
                                            <th style="text-align:center; width:16%">Action</th>
                                        </tr>
                                    </thead>




                                    @foreach (var pair in Model.UnitValidityPairs)
                                    {

                                        if (pair.Key == unit.Id)
                                        {


                                            foreach (var item in pair.Value)
                                            {

                                                @if (unit.SAOrder != null && unit.SAOrder != "")
                                                {
                                                    @if (ItemIdOrder(unit.SAOrder).Contains(item.SummativeId))
                                                    {
                                                        <tr class="table-striped">

                                                            <td>
                                                                @Html.DisplayFor(m => item.Summative.Title)

                                                            </td>
                                                            <td>
                                                                @Html.DisplayFor(m => item.Summative.AssessmentType)

                                                            </td>

                                                            <td>
                                                                @Html.EditorFor(model => item.Open, new { htmlAttributes = new { @class = "form-control", type = "datetime-local" } })

                                                                <span asp-validation-for="SubmissionValidity.Open" class="text-danger"></span>
                                                            </td>
                                                            <td>
                                                                @Html.EditorFor(model => item.Close, new { htmlAttributes = new { @class = "form-control", type = "datetime-local" } })
                                                                <input type="hidden" asp-for="@Model.SubmissionValidity.Id" id="Id" />
                                                                <span asp-validation-for="SubmissionValidity.Open" class="text-danger"></span>
                                                            </td>
                                                            <td style="width:150px">
                                                                <input type="submit" class="btn btn-info form-control" value="Update" />
                                                                <input type="hidden" asp-for="@Model.SubmissionValidity.Id" id="Id" />

                                                            </td>


                                                        </tr>
                                                    }
                                                }




                                            }
                                        }

                                    }

                                }}
                        }}
                    </table>
                </form>
            }
                        else
                        {
                        <p class="text-black-50">No Summative Assignments or Projects exists.</p>
                        }
                    </div>

</div>

