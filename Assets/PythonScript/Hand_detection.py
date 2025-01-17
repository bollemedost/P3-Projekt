import cv2
import numpy as np
import sys
import time  # To measure time

# Function to count the number of spaces (gaps between fingers) in a binary image
def count_spaces(binary_image):
    # Find contours in the binary image
    contours, _ = cv2.findContours(binary_image, cv2.RETR_TREE, cv2.CHAIN_APPROX_SIMPLE)
    if len(contours) == 0:
        return 0  # No contours found, return 0

    # Get the contour with the largest area
    max_contour = max(contours, key=cv2.contourArea)
    
    # Ignore small contours by checking their area
    if cv2.contourArea(max_contour) < 5000:
        return 0

    # Find the convex hull of the largest contour
    hull = cv2.convexHull(max_contour, returnPoints=False)
    if len(hull) < 3:
        return 0  # Not enough points to form a hull

    # Find convexity defects in the contour
    defects = cv2.convexityDefects(max_contour, hull)
    if defects is None:
        return 0  # No defects found

    count = 0  # Initialize gap count
    for i in range(defects.shape[0]):
        s, e, f, d = defects[i, 0]  # Start, end, farthest point, and depth of defect
        start = tuple(max_contour[s][0])
        end = tuple(max_contour[e][0])
        far = tuple(max_contour[f][0])

        # Calculate the lengths of the triangle sides
        a = np.linalg.norm(np.array(end) - np.array(start))
        b = np.linalg.norm(np.array(far) - np.array(start))
        c = np.linalg.norm(np.array(end) - np.array(far))
        
        # Calculate the angle between the triangle sides using the cosine rule
        angle = np.arccos((b ** 2 + c ** 2 - a ** 2) / (2 * b * c))

        # Count a gap if the angle is small enough and the depth is significant
        if angle <= np.pi * 105 / 180 and d > 12000:
            count += 1

    return count + 1  # Include the last gap

# Use DroidCam as the video stream source
cap = cv2.VideoCapture('http://172.20.10.13:4747/video')

# Variables to track consistent detection of a hand sign
consistent_sign = None
consistent_start_time = None
confirmation_time = 3  # Time (in seconds) required for confirmation of an action

while True:
    ret, frame = cap.read()  # Read a frame from the video stream
    if not ret:
        break  # Stop the loop if the frame cannot be read

    # Convert the frame to HSV color space
    hsv = cv2.cvtColor(frame, cv2.COLOR_BGR2HSV)
    # Define skin color range in HSV
    lower_skin = np.array([0, 10, 120], dtype=np.uint8)
    upper_skin = np.array([20, 150, 255], dtype=np.uint8)
    # Create a binary mask for skin detection
    mask = cv2.inRange(hsv, lower_skin, upper_skin)

    # Apply Gaussian blur to smooth the binary mask
    blurred = cv2.GaussianBlur(mask, (7, 7), 0)

    # Apply morphological operations to reduce noise
    kernel = np.ones((5, 5), np.uint8)
    binary = cv2.morphologyEx(blurred, cv2.MORPH_CLOSE, kernel)
    binary = cv2.morphologyEx(binary, cv2.MORPH_OPEN, kernel)

    # Find connected components in the binary image
    num_labels, labels, stats, centroids = cv2.connectedComponentsWithStats(binary, connectivity=8)
    
    # Keep only the largest connected component
    if num_labels > 1:
        largest_label = 1 + np.argmax(stats[1:, cv2.CC_STAT_AREA])
        binary = (labels == largest_label).astype("uint8") * 255
    else:
        binary = np.zeros_like(binary)  # Empty image if no large components found

    # Count the number of fingers detected
    fingers = count_spaces(binary)
    
    # Determine the action based on the number of fingers detected
    action_text = ""
    if fingers == 2:
        action_text = "DoubleJump"
    elif fingers == 3:
        action_text = "Dash"
    elif fingers == 5:
        action_text = "Smash"

    # Check if the same action is detected consistently
    if action_text:
        if consistent_sign == action_text:
            # If already tracking this action, check if it has been held long enough
            if time.time() - consistent_start_time >= confirmation_time:
                print(action_text)  # Output the confirmed action to Unity
                sys.stdout.flush()
                break  # Exit the loop
        else:
            # New action detected, start tracking its duration
            consistent_sign = action_text
            consistent_start_time = time.time()
    else:
        # Reset tracking if no valid action is detected
        consistent_sign = None
        consistent_start_time = None

    # Add information to the frame for debugging purposes
    cv2.putText(frame, f'Fingers: {fingers}', (50, 50), cv2.FONT_HERSHEY_SIMPLEX, 1, (0, 255, 0), 2)
    if consistent_sign:
        cv2.putText(frame, f'Hold: {consistent_sign}', (50, 100), cv2.FONT_HERSHEY_SIMPLEX, 1, (0, 0, 255), 2)

    # Display the original frame and the binary mask
    cv2.imshow("Frame", frame)
    cv2.imshow("Binary", binary)

    # Exit the loop when 'q' is pressed
    if cv2.waitKey(1) & 0xFF == ord('q'):
        break

# Release resources and close OpenCV windows
cap.release()
cv2.destroyAllWindows()



# References:
# https://answers.opencv.org/question/120499/how-to-eliminate-small-contours-in-a-binary-image/
# https://docs.opencv.org/4.x/d5/d45/tutorial_py_contours_more_functions.html
# https://www.codepasta.com/2016/11/06/background-segmentation-removal-with-opencv
# https://www.youtube.com/watch?v=JfaZNiEbreE
# ChatGPT - troubleshooting and errorhandling
