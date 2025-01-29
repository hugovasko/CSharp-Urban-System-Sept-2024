namespace UrbanSystem.Common
{
    public static class ValidationStrings
    {
        public static class Suggestion
        {
            public const string TitleRequiredMessage = "Suggestion title is required!";
            public const string CategoryRequiredMessage = "The category is required!";
            public const string DescriptionRequiredMessage = "Please enter what you want to change!";
            public const string PriorityRequiredMessage = "Please select the priority of the issue!";
            public const string StatusRequiredMessage = "Please select the status of the issue!";
            public const string InvalidUserIdMessage = "Invalid user ID.";
            public const string UserNotFoundMessage = "User not found.";
            public const string UnknownUserMessage = "Unknown user";
            public const string InvalidCitySelectedMessage = "Invalid city selected.";
            public const string InvalidSuggestionIdMessage = "Invalid suggestion ID.";
            public const string SuggestionNotFoundMessage = "Suggestion not found.";
            public const string NotAuthorizedToEditMessage = "You are not authorized to edit this suggestion.";
            public const string NotAuthorizedToDeleteMessage = "You are not authorized to delete this suggestion.";
        }

        public static class Location
        {
            public const string CityNameRequiredMessage = "City name is required!";
            public const string CityNameMaxLengthMessage = "City name cannot exceed 30 characters!";
            public const string StreetNameRequiredMessage = "Street name is required!";
            public const string StreetNameMaxLengthMessage = "Street name cannot exceed 20 characters!";
            public const string CityPictureRequiredMessage = "City picture URL is required!";
            public const string CityPictureDisplayMessage = "City picture URL";
            public const string UrlErrorMessage = "Please enter a valid URL!";
            public const string UnknownLocation = "Unknown";
        }

        public static class Project
        {
            public const string NameLengthErrorMessage = "Name must be less than 100 characters.";
            public const string DesiredSumRangeErrorMessage = "Desired sum must be between 0.01 and 10,000,000.";
            public const string ImageUrlLengthErrorMessage = "Image URL must be less than 2048 characters.";
            public const string DescriptionLengthErrorMessage = "Description must be less than 500 characters.";

            public const string ErrorFetchingAllProjects = "Error occurred while fetching all projects";
            public const string ProcessingRequestError = "An error occurred while processing your request.";
            public const string InvalidProjectId = "Invalid project ID.";
            public const string ProjectNotFoundError = "The requested project could not be found.";
            public const string FetchProjectDetailsError = "Error occurred while fetching project details for ID: {0}";
            public const string AddProjectFormError = "Error occurred while preparing the Add Project form";
            public const string FetchCityListError = "Error occurred while fetching city list for invalid Add Project form";
            public const string NewProjectAdded = "New project added successfully";
            public const string AddProjectError = "Error occurred while adding a new project";
            public const string AddProjectRetryError = "An error occurred while adding the project. Please try again.";
            public const string DeleteNonExistentProjectWarning = "The project you're trying to delete could not be found.";
            public const string ProjectDeletedSuccessfully = "Project deleted successfully: {0}";
            public const string DeleteProjectError = "Error occurred while deleting project: {0}";
            public const string InvalidOperationWhileDeletingProject = "Invalid operation while deleting project: {0}";
            public const string DeleteProjectUnexpectedError = "An unexpected error occurred while deleting the project.";
        }

        public static class Formatting
        {
            public const string DateDisplayFormat = "dd/MM/yyyy HH:mm";
            public const string DurationFormat = "hh\\:mm";
            public const string SuggestionUploadedOnFormat = "dd/MM/yyyy HH:mm";
        }

        public static class Sorting
        {
            public const string SortByTitleMessage = "title";
            public const string SortByDateMessage = "date";
        }

        public static class Meeting
        {
            // meeting controller
            public const string ErrorFetchingAllMeetings = "Error occurred while fetching all meetings";
            public const string ErrorProcessingRequest = "An error occurred while processing your request.";
            public const string ErrorPreparingAddMeetingForm = "Error occurred while preparing the Add Meeting form";
            public const string ErrorRepopulatingAddMeetingForm = "Error occurred while repopulating the Add Meeting form";
            public const string NewMeetingCreatedByUser = "New meeting created by user {0}";
            public const string ErrorCreatingNewMeeting = "Error occurred while creating a new meeting";
            public const string InvalidMeetingId = "Invalid meeting ID.";
            public const string ErrorPreparingEditMeeting = "Error occurred while preparing to edit meeting {0}";
            public const string ErrorUpdatingMeeting = "Error occurred while updating meeting {0}";
            public const string MeetingUpdatedByUser = "Meeting {MeetingId} updated by user {0}";
            public const string UnauthorizedEditAttempt = "Unauthorized edit attempt for meeting {0} by user {1}";
            public const string ErrorPreparingDeleteMeeting = "Error occurred while preparing to delete meeting {0}";
            public const string UnauthorizedDeleteAttempt = "Unauthorized delete attempt for meeting {0} by user {1}";
            public const string ErrorDeletingMeeting = "Error occurred while deleting meeting {0}";
            public const string ErrorDeletingMeetingMessage = "An error occurred while deleting the meeting. Please try again.";
            public const string ErrorFetchingMeetingDetails = "Error occurred while fetching details for meeting {0}";
            public const string ErrorAttendingMeeting = "Error occurred while user {0} was attempting to attend meeting {1}";
            public const string UserRegisteredForMeeting = "User {0} registered for meeting {1}";
            public const string SuccessfullyRegisteredForMeeting = "You have successfully registered for the meeting!";
            public const string ErrorCancelingAttendance = "Error occurred while user {0} was attempting to cancel attendance for meeting {1}";
            public const string UserCancelledAttendanceForMeeting = "User {0} cancelled attendance for meeting {1}";
            public const string SuccessfullyCancelledAttendance = "You have successfully cancelled your attendance.";
            public const string ErrorFetchingAttendedMeetings = "Error occurred while fetching attended meetings for user {0}";

            // meeting service
            public const string InvalidLocationId = "Invalid location ID.";
            public const string OrganizerNotFound = "Organizer not found.";
            public const string MeetingNotFound = "Meeting not found.";
            public const string UserNotFound = "User not found.";
            public const string AlreadyAttendingMeeting = "You are already attending this meeting.";
            public const string NotAttendingMeeting = "You are not attending this meeting.";
            public const string InvalidLocation = "Invalid location ID.";
            public const string InvalidMeeting = "Meeting not found";
            public const string MeetingNotFoundForDelete = "Meeting not found for delete.";
            public const string InvalidMeetingForEdit = "Invalid meeting ID for edit.";

            public const string IdRequired = "Id is required";
            public const string TitleRequired = "Title is required";
            public const string TitleMaxLength = "Title cannot be longer than 200 characters";
            public const string DescriptionRequired = "Description is required";
            public const string DescriptionMaxLength = "Description cannot be longer than 1000 characters";
            public const string ScheduledDateRequired = "Scheduled date is required";
            public const string ScheduledDateDisplayName = "Scheduled Date";
            public const string DurationRequired = "Duration is required";
            public const string DurationDisplayName = "Duration (hours)";
            public const string DurationRange = "Duration must be between 0.5 and 8 hours";
            public const string LocationRequired = "Location is required";
        }

        public static class Error
        {
            public const string PageNotFoundMessage = "Sorry, the page you requested could not be found";
            public const string GeneralErrorMessage = "Sorry, something went wrong on our end";
            public const string NotFoundView = "NotFound";
            public const string ErrorView = "Error";
        }

        public static class Home
        {
            public const string TitleKey = "Title";
            public const string TitleValue = "Home Page";
            public const string MessageKey = "Message";
            public const string WelcomeMessage = "Welcome to the Urban System web app!";
        }

        public static class MySuggestionControllerMessages
        {
            public const string RedirectToLoginPage = "/Identity/Account/Login";
        }

        public static class ProjectControllerMessages
        {
            public const string FetchAllProjectsError = "Error occurred while fetching all projects";
            public const string ProcessingRequestError = "An error occurred while processing your request.";
            public const string InvalidProjectId = "Invalid project ID.";
            public const string ProjectNotFoundError = "The requested project could not be found.";
            public const string FetchProjectDetailsError = "Error occurred while fetching project details for ID: {0}";
            public const string AddProjectFormError = "Error occurred while preparing the Add Project form";
            public const string FetchCityListError = "Error occurred while fetching city list for invalid Add Project form";
            public const string NewProjectAdded = "New project added successfully";
            public const string AddProjectError = "Error occurred while adding a new project";
            public const string AddProjectRetryError = "An error occurred while adding the project. Please try again.";
            public const string DeleteNonExistentProjectWarning = "The project you're trying to delete could not be found.";
            public const string ProjectDeletedSuccessfully = "Project deleted successfully: {0}";
            public const string DeleteProjectError = "Error occurred while deleting project: {0}";
            public const string InvalidOperationWhileDeletingProject = "Invalid operation while deleting project: {0}";
            public const string DeleteProjectUnexpectedError = "An unexpected error occurred while deleting the project.";
        }

        public static class LocationControllerMessages
        {
            public const string FetchAllLocationsError = "Error occurred while fetching all locations";
            public const string AddLocationError = "Error occurred while adding a new location";
            public const string AddLocationRetryError = "An error occurred while adding the location. Please try again.";
            public const string InvalidLocationIdError = "Invalid location ID.";
            public const string LocationNotFoundError = "The requested location could not be found.";
            public const string FetchLocationDetailsError = "Error occurred while fetching location details for ID: {LocationId}";
            public const string GeneralProcessingError = "An error occurred while processing your request.";

            public const string LocationAddedLog = "New location added successfully";
            public const string LocationNotFoundLog = "Location not found: {LocationId}";
        }

        public static class SuggestionControllerMessages
        {
            public const string InvalidSuggestionId = "Invalid suggestion ID.";
            public const string InvalidSuggestionOrCommentContent = "Invalid suggestion ID or comment content.";
            public const string AddSuggestionError = "Failed to add comment. Please try again.";
            public const string SuggestionNotFoundError = "The requested suggestion could not be found.";
            public const string AddSuggestionLog = "User {0} added a new suggestion.";
            public const string RetrieveSuggestionForEditError = "You do not have permission to edit this suggestion.";
            public const string RetrieveSuggestionForEditLog = "Failed to retrieve suggestion for edit. ID: {0}, User: {1}. Error: {2}";
            public const string UpdateSuggestionError = "Failed to update the suggestion. Please try again.";
            public const string UpdateSuggestionLog = "User {0} updated suggestion {1}.";
            public const string SuggestionUpdateSuccess = "Suggestion updated successfully.";
            public const string RetrieveSuggestionForDeleteError = "You do not have permission to delete this suggestion.";
            public const string RetrieveSuggestionForDeleteLog = "Failed to retrieve suggestion for delete confirmation. ID: {0}, User: {1}. Error: {2}";
            public const string DeleteSuggestionError = "Failed to delete the suggestion. Please try again.";
            public const string DeleteSuggestionLog = "User {0} deleted suggestion {1}.";
            public const string SuggestionDeleteSuccess = "Suggestion deleted successfully.";
        }

        public static class Funding
        {
            public const string AmountGreaterThanZero = "Amount must be greater than 0.";
            public const string ConfirmationDisplayName = "I confirm this funding amount";
        }
    }
}
