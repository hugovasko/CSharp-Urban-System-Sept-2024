// wwwroot/js/ratings.js
function rateProject(projectId, score) {
    const token = document.querySelector('input[name="__RequestVerificationToken"]').value;

    fetch(`/api/Rating/project/${projectId}`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken': token,
            'X-CSRF-TOKEN': token
        },
        body: JSON.stringify(score)
    })
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return response.json();
        })
        .then(data => {
            const ratingElement = document.querySelector(`#project-rating-${projectId}`);
            if (ratingElement) {
                ratingElement.textContent = parseFloat(data.averageRating).toFixed(1);
            }
        })
        .catch(error => {
            console.error('Error:', error);
            alert('Failed to submit rating. Please try again.');
        });
}