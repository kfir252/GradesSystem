import random
from faker import Faker
import pandas as pd

# Initialize Faker
fake = Faker()

# Define the number of student records to generate
num_records = 100

# Generate random student data
student_data = {
    "Name": [fake.first_name() for _ in range(num_records)],
    "LastName": [fake.last_name() for _ in range(num_records)],
    "ID": [fake.unique.random_number(digits=8) for _ in range(num_records)],
    "Year": [random.randint(1, 4) for _ in range(num_records)],
    "Hw1 - 2%": [random.randint(0, 100) for _ in range(num_records)],
    "Hw2 - 2%": [random.randint(0, 100) for _ in range(num_records)],
    "Hw3 - 2%": [random.randint(0, 100) for _ in range(num_records)],
    "Hw4 - 2%": [random.randint(0, 100) for _ in range(num_records)],
    "Hw5 - 2%": [random.randint(0, 100) for _ in range(num_records)],
    "Hw6 - 2%": [random.randint(0, 100) for _ in range(num_records)],
    "Hw7 - 2%": [random.randint(0, 100) for _ in range(num_records)],
    "Hw8 - 2%": [random.randint(0, 100) for _ in range(num_records)],
    "Exam A - 42%": [random.randint(0, 100) for _ in range(num_records)],
    "Exam B - 42%": [random.randint(0, 100) for _ in range(num_records)]
}

# Create a DataFrame
student_df = pd.DataFrame(student_data)

# Define output file path
output_file_path = "LinearAlgebra 2024.csv"

# Save to CSV
student_df.to_csv(output_file_path, index=False)